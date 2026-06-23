using AutoMapper;
using CloudinaryDotNet.Actions;
using Health.Domain.Contracts;
using Health.Domain.Entities;
using Health.Domain.Entities.DoctorModule;
using Health.Domain.Entities.IdentityModule;
using Health.Domain.Entities.PatientModule;
using Health.Services.Abstraction.IdentityModule;
using Health.Services.Abstraction.IdentityModuleAbstraction;
using Health.Services.Specifications.DoctorSpecification;
using Health.Services.Specifications.PatientSpecification;
using Health.Shared.CommonResponses;
using Health.Shared.DTOs.Enums;
using Health.Shared.DTOs.IdentityDTOs;
using Health.Shared.DTOs.IdentityDTOs.Requests;
using Health.Shared.DTOs.IdentityDTOs.Responses;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.Data;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;
using SendGrid.Helpers.Mail;
using System.IdentityModel.Tokens.Jwt;
using System.Numerics;
using System.Security.Claims;
using System.Security.Cryptography;
using System.Text;
using System.Text.RegularExpressions;
using Error = Health.Shared.CommonResponses.Error;
using ForgotPasswordRequest = Health.Shared.DTOs.IdentityDTOs.Requests.ForgotPasswordRequest;
using Gender = Health.Domain.Entities.DoctorModule.Gender;
using LoginRequest = Health.Shared.DTOs.IdentityDTOs.Requests.LoginRequest;
using ResetPasswordRequest = Health.Shared.DTOs.IdentityDTOs.Requests.ResetPasswordRequest;

namespace Health.Services.ServicesImplementation.IdentityModule
{
    public class AuthenticationService : IAuthenticationService
    {

        private readonly UserManager<ApplicationUser> _userManager;
        private readonly IConfiguration _configuration;
        private readonly IUnitOfWork _unitOfWork;
        private readonly IMapper _mapper;
        private readonly IAttachmentService _attachmentService;
        private readonly IOtpService _otpService;
        private readonly IProfileCompletionService _profileCompletionService;

        public AuthenticationService(
            UserManager<ApplicationUser> userManager,
            IConfiguration configuration,
            IUnitOfWork unitOfWork,
            IMapper mapper,
            IAttachmentService attachmentService,
            IOtpService otpService,
            IProfileCompletionService profileCompletionService)
        {
            _userManager = userManager;
            _configuration = configuration;
            _unitOfWork = unitOfWork;
            _mapper = mapper;
            _attachmentService = attachmentService;
            _otpService = otpService;
            _profileCompletionService = profileCompletionService;
        }

        public async Task<Result<RegisterResponse>> RegisterDoctorAsync(RegisterDoctorRequest request)
        {
            var validationError = await ValidateEmailAndPhoneAsync(request.Email, request.PhoneNumber);

            if (validationError != null)
                return validationError;

            var user = _mapper.Map<ApplicationUser>(request);
            var identityResult = await _userManager.CreateAsync(user, request.Password);
            if (!identityResult.Succeeded)
                return identityResult.Errors.Select(e => Error.Validation(e.Code, e.Description)).ToList();

            await _userManager.AddToRoleAsync(user, "Doctor");

            var doctorProfile = _mapper.Map<DoctorProfile>(request);
            doctorProfile.UserId = user.Id;


            try
            {
                await _unitOfWork.GetRepository<DoctorProfile, int>().AddAsync(doctorProfile);
                await _unitOfWork.SaveChanges();
            }
            catch (Exception ex)
            {
                await _userManager.DeleteAsync(user);
                return Error.Failure("DoctorProfile.CreateFailed", ex.Message);
            }
            var refreshToken = await GenerateRefreshTokenAsync(user);
            var accessToken = await CreateTokenAsync(user);


            var userId = await GetUserProfileIdAsync(user.Id, "Doctor");

            var registerResponse = new RegisterResponse(
                 5,
                 "Doctor",
                 "Account Created Succesffully",
                 new TokenResponse(
                     accessToken,
                     refreshToken.Token,
                     refreshToken.ExpiresOn
                 )
            );

            return registerResponse;
        }

        public async Task<Result<RegisterResponse>> RegisterPatientAsync(RegisterPatientRequest request)
        {
            var validationError = await ValidateEmailAndPhoneAsync(request.Email, request.PhoneNumber);

            if (validationError != null)
                return validationError;

            var user = _mapper.Map<ApplicationUser>(request);

            var identityResult = await _userManager.CreateAsync(user, request.Password);
            if (!identityResult.Succeeded)
                return identityResult.Errors.Select(e => Error.Validation(e.Code, e.Description)).ToList();

            await _userManager.AddToRoleAsync(user, "Patient");

            var patientProfile = _mapper.Map<PatientProfile>(request);
            patientProfile.UserId = user.Id;

            try
            {
                await _unitOfWork.GetRepository<PatientProfile, int>().AddAsync(patientProfile);
                await _unitOfWork.SaveChanges();
            }
            catch (Exception ex)
            {
                await _userManager.DeleteAsync(user);
                return Error.Failure("PatientProfile.CreateFailed", ex.Message);
            }

            var refreshToken = await GenerateRefreshTokenAsync(user);
            var accessToken = await CreateTokenAsync(user);

            var userId = await GetUserProfileIdAsync(user.Id, "Patient");

            var registerResponse = new RegisterResponse(
                5,
                "Doctor",
                "Account Created Succesffully",
                new TokenResponse(
                    accessToken,
                    refreshToken.Token,
                    refreshToken.ExpiresOn
                )
            );

            return registerResponse;
        }

        public async Task<Result<LoginResponse>> LoginAsync(LoginRequest request)
        {
            var user = await _userManager.FindByEmailAsync(request.Email);
            if (user == null)
                return Error.InvalidCredentials("User.InvalidEmail");

            var isPasswordValid = await _userManager.CheckPasswordAsync(user, request.Password);
            if (!isPasswordValid)
                return Error.InvalidCredentials("User.InvalidPassword");

            var roles = await _userManager.GetRolesAsync(user);
            var role = roles.First();

            var refreshToken = await GenerateRefreshTokenAsync(user);
            var accessToken = await CreateTokenAsync(user);

            var userId = await GetUserProfileIdAsync(user.Id, role);

            var persentage = await _profileCompletionService.CalculateAndUpdateDoctorStatusAsync(user.Id);

            var loginRes = new LoginResponse(
                accessToken,
                refreshToken.Token,
                refreshToken.ExpiresOn,
                new CurrentUserResponse(
                    userId.Value,
                    $"{user.FirstName} {user.LastName}",
                    user.Email!,
                    role
                    )
                );
            return loginRes;
        }

        public async Task<Result<TokenResponse>> RefreshTokenAsync(string refreshToken)
        {
            var user = await _userManager.Users.Include(x => x.RefreshTokens)
                                         .FirstOrDefaultAsync(
                                            x => x.RefreshTokens.Any(x => x.Token == refreshToken &&
                                            x.RevokeOn == null &&
                                            x.ExpiresOn > DateTime.UtcNow));

            if (user is null)
                return Error.Unauthorized("Invalid.RefreshToken");

            var oldToken = user.RefreshTokens.First(x => x.Token == refreshToken);
            oldToken.RevokeOn = DateTime.UtcNow;

            var newRefreshToken = await GenerateRefreshTokenAsync(user);
            var accessToken = await CreateTokenAsync(user);
            var displayName = $"{user.FirstName} {user.LastName}";
            var role = (await _userManager.GetRolesAsync(user)).FirstOrDefault();

            if (role is null)
                return Error.Unauthorized("Invalid.Role");

            var userId = await GetUserProfileIdAsync(user.Id, role);

            var tokenRes = new TokenResponse(
                accessToken,
                newRefreshToken.Token,
                newRefreshToken.ExpiresOn
                );

            return tokenRes;
        }

        public async Task<Result<CommandResponse>> ForgetPasswordAsync(ForgotPasswordRequest request)
        {
            var user = await _userManager.FindByEmailAsync(request.Email);
            if (user is null)
                return Error.Unauthorized("Auth.UserNotFound", "No account found with this email.");

            var result = await _otpService.SendOtpAsync(request.Email, OtpPurposeDTO.ForgotPassword);

            if (!result.IsSuccess)
                return Error.Failure("Failed.ForgetPassword", "Error In Sending Email Try Again");

            return new CommandResponse(true, "If the account exists, an OTP has been sent to the registered email address.");
        }

        public async Task<Result<CommandResponse>> VerifyOtpAsync(VerifyOtpRequest request)
        {
            var user = await _userManager.FindByEmailAsync(request.Email);
            if (user is null)
                return Error.Unauthorized("Auth.UserNotFound", "No account found with this email.");

            var result = await _otpService.VerifyOtpAsync(request.Email, request.OtpCode, OtpPurposeDTO.ForgotPassword);
            if (result.IsFailure)
                return Error.Failure("Failed.VerifyOtp", "OTP is not correct, Try again");

            var markedVerify = await _otpService.MarkOtpVerifiedAsync(request.Email, OtpPurposeDTO.ForgotPassword);
            if (markedVerify.IsFailure)
                return Error.Failure("Failed.markedVerify", "OTP is not correct, Try again");

            return new CommandResponse(true, "The OTP has been verified successfully.");
        }

        public async Task<Result<CommandResponse>> ResendOtpAsync(ResendOtpRequest request)
        {
            var user = await _userManager.FindByEmailAsync(request.Email);
            if (user is null)
                return Error.Unauthorized("Auth.UserNotFound", "No account found with this email.");

            var deleteResult = await _otpService.DeleteOtpAsync(request.Email, OtpPurposeDTO.ForgotPassword);
            if (deleteResult is null)
                return Error.Failure("Failed.ResendOtp", "Error In Resending Otp Try Again");

            var sendResult = await _otpService.SendOtpAsync(request.Email, OtpPurposeDTO.ForgotPassword);
            if (sendResult is null)
                return Error.Failure("Failed.ResendOtp", "Error In Resending Otp Try Again");

            return new CommandResponse(true, "A new OTP has been sent successfully.");
        }

        public async Task<Result<CommandResponse>> ResetPasswordAsync(ResetPasswordRequest request)
        {
            if (request.Password != request.ConfirmPassword)
            {
                return Error.Validation("Auth.PasswordMismatch", "Passwords do not match.");
            }

            var isVerifiedResult = await _otpService.IsOtpVerifiedAsync(request.Email, OtpPurposeDTO.ForgotPassword);
            if (isVerifiedResult.IsFailure || !isVerifiedResult.Value)
                return Error.Failure("Auth.OtpNotVerified", "OTP not verified.");

            var user = await _userManager.FindByEmailAsync(request.Email);
            if (user is null)
                return Error.Unauthorized("Auth.UserNotFound", "No account found with this email.");

            var resetToken = await _userManager.GeneratePasswordResetTokenAsync(user);

            var result = await _userManager.ResetPasswordAsync(user, resetToken, request.Password);
            if (!result.Succeeded)
                return Error.Failure("Auth.PasswordResetFailed", "Password reset failed.");

            await _otpService.DeleteOtpAsync(request.Email, OtpPurposeDTO.ForgotPassword);

            return new CommandResponse(true, "Your password has been successfully reset.");
        }

        public async Task<Result<CommandResponse>> ChangePasswordAsync(string userId, ChangePasswordRequest request)
        {
            if (request.Password != request.ConfirmPassword)
                return Error.Validation("User.PasswordMismatch", "Passwords do not match.");

            var user = await _userManager.FindByIdAsync(userId);
            if (user is null)
                return Error.NotFound("User.NotFound", "User not found.");

            var removePassword = await _userManager.RemovePasswordAsync(user);
            if (!removePassword.Succeeded)
                return Error.Failure("User.PasswordRemoveFailed", "Failed to update password.");

            var addResult = await _userManager.AddPasswordAsync(user, request.Password);
            if (!addResult.Succeeded)
                return Error.Failure("User.PasswordChangeFailed", "Failed to update password.");

            return new CommandResponse(true, "The password has been updated successfully.");
        }

        public async Task<Result<DoctorProfileResponse>> UpdateDoctorProfileAsync(string userId, UpdateDoctorProfileRequest request)
        {
            var userResult = await GetAndUpdateDoctorAsync(userId, request);
            if (userResult.IsFailure)
                return userResult.Errors.First();

            var user = userResult.Value!;

            var userUpdateResult = await _userManager.UpdateAsync(user);
            if (!userUpdateResult.Succeeded)
            {
                var error = userUpdateResult.Errors.First();
                return Error.Failure(error.Code, error.Description);
            }

            var doctorResult = await GetAndUpdateDoctorProfileAsync(userId, request);
            if (doctorResult.IsFailure)
                return doctorResult.Errors.First();

            var doctorProfile = doctorResult.Value!;

            try
            {
                _unitOfWork.GetRepository<DoctorProfile, int>().Update(doctorProfile);
                await _unitOfWork.SaveChanges();
            }
            catch (Exception ex)
            {
                return Error.Failure("DoctorProfile.UpdateFailed", ex.Message);
            }

            var accountPercentage = await _profileCompletionService.CalculateAndUpdateDoctorStatusAsync(user.Id);

            var doctorRes = new DoctorProfileResponse(
                doctorProfile.Id,
                doctorProfile.DisplayName,
                doctorProfile.Bio,
                doctorProfile.Specialization,
                doctorProfile.YearsOfExperience,
                accountPercentage.Value
                );

            return doctorRes;
        }

        public async Task<Result<CommandResponse>> UpdatePatientProfileAsync(string userId, UpdatePatientProfileRequest request)
        {
            var result = await GetAndUpdatePatientProfileAsync(userId, request);
            if (!result.IsSuccess)
                return result.Errors.First();

            try
            {
                await _unitOfWork.SaveChanges();
            }
            catch (Exception ex)
            {
                return Error.Failure("PatientProfile.UpdateFailed", ex.Message);
            }

            return new CommandResponse(true, "Your profile has been updated successfully.");
        }


        #region HelperMethod

        private async Task<Error?> ValidateEmailAndPhoneAsync(string email, string phoneNumber)
        {
            var existingEmail = await _userManager.FindByEmailAsync(email);
            if (existingEmail != null)
                return Error.Validation("Email.Exists", "Email already exists.");

            if (!string.IsNullOrWhiteSpace(phoneNumber))
            {
                var phoneExists = await _userManager.Users
                    .AnyAsync(u => u.PhoneNumber == phoneNumber);

                if (phoneExists)
                    return Error.Validation("Phone.Exists", "Phone number already exists.");
            }

            return null;
        }

        private async Task<Result<string>> UploadImageMandatoryAsync(IFormFile file, string folder)
        {
            if (file == null)
                return Error.Validation("File.Required", $"File in folder '{folder}' is required.");

            var result = await _attachmentService.UploadImageAsync(file, folder);
            if (!result.IsSuccess)
                return Result<string>.Fail(result.Errors.ToList());

            return Result<string>.Ok(result.Value);
        }

        private async Task<Result<DoctorProfile>> GetAndUpdateDoctorProfileAsync(string userId, UpdateDoctorProfileRequest request)
        {
            var spec = new DoctorByIdSpecification(userId);
            var doctorProfile = await _unitOfWork
                .GetRepository<DoctorProfile, int>()
                .GetByIdAsync(spec);

            if (doctorProfile is null)
                return Error.NotFound("DoctorProfile.NotFound", "Doctor profile not found.");

            doctorProfile.Specialization = request.Specialization ?? doctorProfile.Specialization;
            doctorProfile.Bio = request.Bio ?? doctorProfile.Bio;
            doctorProfile.ClinicLocation = request.ClinicLocation ?? doctorProfile.ClinicLocation;
            doctorProfile.PhoneClinc = request.PhoneClinic ?? doctorProfile.PhoneClinc;

            if (request.YearsOfExperience.HasValue)
                doctorProfile.YearsOfExperience = request.YearsOfExperience.Value;

            if (request.Address is not null)
                doctorProfile.Address = _mapper.Map<Address>(request.Address);

            if (request.Gender.HasValue)
                doctorProfile.Gender = (Gender)request.Gender.Value;

            if (request.DoctorPictureFile is not null)
            {
                if (!string.IsNullOrEmpty(doctorProfile.DoctorPictureUrl))
                    await _attachmentService.DeleteImageAsync(doctorProfile.DoctorPictureUrl);

                var pictureResult = await UploadImageMandatoryAsync(request.DoctorPictureFile, "doctors");
                if (!pictureResult.IsSuccess)
                    return Error.Validation("PictureUpdate.Failed", "Picture update failed");

                doctorProfile.DoctorPictureUrl = pictureResult.Value!;
            }

            if (request.SyndicateCardFile is not null)
            {
                if (!string.IsNullOrEmpty(doctorProfile.SyndicateCardUrl))
                    await _attachmentService.DeleteImageAsync(doctorProfile.SyndicateCardUrl);

                var syndicateResult = await UploadImageMandatoryAsync(request.SyndicateCardFile, "doctors");
                if (!syndicateResult.IsSuccess)
                    return Error.Validation("SyndicateCardUpdate.Failed", "Syndicate card update failed");

                doctorProfile.SyndicateCardUrl = syndicateResult.Value!;
            }

            return Result<DoctorProfile>.Ok(doctorProfile);
        }

        private async Task<Result<ApplicationUser>> GetAndUpdateDoctorAsync(string userId, UpdateDoctorProfileRequest request)
        {
            var user = await _userManager.FindByIdAsync(userId);
            if (user is null)
                return Error.NotFound("User.NotFound", "User not found.");

            user.FirstName = request.FirstName ?? user.FirstName;
            user.LastName = request.LastName ?? user.LastName;
            user.PhoneNumber = request.PhoneNumber ?? user.PhoneNumber;

            var updateResult = await _userManager.UpdateAsync(user);
            if (!updateResult.Succeeded)
            {
                var error = updateResult.Errors.First();
                return Error.Failure(error.Code, error.Description);
            }

            return Result<ApplicationUser>.Ok(user);
        }

        private async Task<Result> GetAndUpdatePatientProfileAsync(string userId, UpdatePatientProfileRequest request)
        {
            var user = await _userManager.FindByIdAsync(userId);
            if (user is null)
                return Result.Fail(Error.NotFound("User.NotFound", "User not found."));

            var spec = new PatientByIdSpecification(userId);
            var patientProfile = await _unitOfWork
                .GetRepository<PatientProfile, int>()
                .GetByIdAsync(spec);

            if (patientProfile is null)
                return Result.Fail(Error.NotFound("PatientProfile.NotFound", "Patient profile not found."));

            if (!string.IsNullOrWhiteSpace(request.FirstName)) user.FirstName = request.FirstName;
            if (!string.IsNullOrWhiteSpace(request.LastName)) user.LastName = request.LastName;
            if (!string.IsNullOrWhiteSpace(request.PhoneNumber)) user.PhoneNumber = request.PhoneNumber;

            var userUpdateResult = await _userManager.UpdateAsync(user);
            if (!userUpdateResult.Succeeded)
            {
                var error = userUpdateResult.Errors.First();
                return Result.Fail(Error.Failure(error.Code, error.Description));
            }

            patientProfile.DisplayName = request.DisplayName ?? patientProfile.DisplayName;
            if (request.Gender.HasValue) patientProfile.Gender = (Gender)request.Gender.Value;
            if (request.DateOfBirth.HasValue) patientProfile.DateOfBirth = request.DateOfBirth.Value;
            if (request.Address is not null)
                patientProfile.Address = _mapper.Map<Address>(request.Address);

            if (request.ChronicDiseaseIds is not null)
            {
                var chronicResult = await UpdatePatientChronicDiseases(patientProfile, request.ChronicDiseaseIds);
                if (!chronicResult.IsSuccess)
                    return chronicResult;
            }
            else
            {
                patientProfile.PatientChronicDiseases.Clear();
            }

            _unitOfWork.GetRepository<PatientProfile, int>().Update(patientProfile);

            return Result.Ok();
        }

        private async Task<Result> UpdatePatientChronicDiseases(PatientProfile patientProfile, ICollection<int> diseaseIds)
        {
            if (!diseaseIds.Any())
            {
                patientProfile.PatientChronicDiseases.Clear();
                return Result.Ok();
            }

            var validDiseases = await _unitOfWork
                .GetRepository<ChronicDisease, int>()
                .GetAllAsync(new ChronicDiseasesByIdsSpecification(diseaseIds));

            var validDiseaseIds = validDiseases.Select(d => d.Id).ToList();

            var invalidIds = diseaseIds.Except(validDiseaseIds).ToList();
            if (invalidIds.Any())
            {
                return Result.Fail(Error.Validation(
                    "ChronicDisease.InvalidIds",
                    $"Invalid chronic disease IDs: {string.Join(", ", invalidIds)}"));
            }

            patientProfile.PatientChronicDiseases
                .Where(pc => !validDiseaseIds.Contains(pc.ChronicDiseaseId))
                .ToList()
                .ForEach(pc => patientProfile.PatientChronicDiseases.Remove(pc));

            foreach (var diseaseId in validDiseaseIds)
            {
                if (!patientProfile.PatientChronicDiseases.Any(pc => pc.ChronicDiseaseId == diseaseId))
                {
                    patientProfile.PatientChronicDiseases.Add(new PatientChronicDisease
                    {
                        PatientId = patientProfile.Id,
                        ChronicDiseaseId = diseaseId
                    });
                }
            }

            return Result.Ok();
        }

        private async Task<string> CreateTokenAsync(ApplicationUser user)
        {
            // Token [Issuer, Audience, Claims, Expires, Signing Credentials]

            var claims = new List<Claim>
            {
                new Claim(ClaimTypes.NameIdentifier, user.Id),
                new Claim(ClaimTypes.Name, $"{user.FirstName!} {user.LastName!}"),
                new Claim(JwtRegisteredClaimNames.Email, user.Email!),
                new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString()),
            };

            var roles = await _userManager.GetRolesAsync(user);

            foreach (var role in roles)
                claims.Add(new Claim(ClaimTypes.Role, role));

            var secretKey = _configuration["JWTOptions:SecretKey"];
            var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(secretKey!));
            var cred = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

            var token = new JwtSecurityToken(
                issuer: _configuration["JWTOptions:Issuer"],
                audience: _configuration["JWTOptions:Audience"],
                claims: claims,
                expires: DateTime.UtcNow.AddDays(1),
                signingCredentials: cred
                );

            return new JwtSecurityTokenHandler().WriteToken(token);
        }

        private async Task<RefreshToken> GenerateRefreshTokenAsync(ApplicationUser user, int expireDays = 10)
        {
            var refreshToken = new RefreshToken()
            {
                Token = Convert.ToBase64String(RandomNumberGenerator.GetBytes(32)),
                CreatedOn = DateTime.UtcNow,
                ExpiresOn = DateTime.UtcNow.AddDays(expireDays)
            };

            user.RefreshTokens.Add(refreshToken);
            await _userManager.UpdateAsync(user);

            return refreshToken;
        }

        private async Task<Result<int>> GetUserProfileIdAsync(string userId, string role)
        {
            return role switch
            {
                "Doctor" => await GetDoctorProfileId(userId),
                "Patient" => await GetPatientProfileId(userId),
                _ => Result<int>.Fail(Error.Validation("Role.Invalid", "Unsupported user role"))
            };
        }

        private async Task<Result<int>> GetPatientProfileId(string userId)
        {
            var spec = new PatientByIdSpecification(userId);

            var patient = await _unitOfWork.GetRepository<PatientProfile, int>().GetByIdAsync(spec);

            if (patient is null)
                return Result<int>.Fail(Error.NotFound("Patient.NotFound", "Patient profile not found"));

            return Result<int>.Ok(patient.Id);
        }

        private async Task<Result<int>> GetDoctorProfileId(string userId)
        {
            var spec = new DoctorByIdSpecification(userId);

            var doctor = await _unitOfWork.GetRepository<DoctorProfile, int>().GetByIdAsync(spec);

            if (doctor is null)
                return Result<int>.Fail(Error.NotFound("Doctor.NotFound", "Doctor profile not found"));

            return Result<int>.Ok(doctor.Id);
        }
        #endregion

    }
}
