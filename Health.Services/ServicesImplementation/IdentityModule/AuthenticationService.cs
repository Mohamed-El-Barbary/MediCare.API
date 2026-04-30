using AutoMapper;
using Health.Domain.Contracts;
using Health.Domain.Entities;
using Health.Domain.Entities.DoctorModule;
using Health.Domain.Entities.IdentityModule;
using Health.Domain.Entities.PatientModule;
using Health.Services.Abstraction.IdentityModuleAbstraction;
using Health.Services.Specifications.DoctorSpecification;
using Health.Services.Specifications.PatientSpecification;
using Health.Shared.CommonResponses;
using Health.Shared.DTOs.Enums;
using Health.Shared.DTOs.IdentityDTOs;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Security.Cryptography;
using System.Text;
using Gender = Health.Domain.Entities.DoctorModule.Gender;

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

        public AuthenticationService(
            UserManager<ApplicationUser> userManager,
            IConfiguration configuration,
            IUnitOfWork unitOfWork,
            IMapper mapper,
            IAttachmentService attachmentService,
            IOtpService otpService)
        {
            _userManager = userManager;
            _configuration = configuration;
            _unitOfWork = unitOfWork;
            _mapper = mapper;
            _attachmentService = attachmentService;
            _otpService = otpService;
        }

        public async Task<Result<UserDTO>> RegisterDoctorAsync(RegisterDoctorDTO registerDTO)
        {
            var validationError = await ValidateEmailAndPhoneAsync(registerDTO.Email, registerDTO.PhoneNumber);

            if (validationError != null)
                return validationError;

            var user = _mapper.Map<ApplicationUser>(registerDTO);
            var identityResult = await _userManager.CreateAsync(user, registerDTO.Password);
            if (!identityResult.Succeeded)
                return identityResult.Errors.Select(e => Error.Validation(e.Code, e.Description)).ToList();

            await _userManager.AddToRoleAsync(user, "Doctor");

            var doctorPicResult = await UploadImageMandatoryAsync(registerDTO.DoctorPictureFile, "doctors");
            if (!doctorPicResult.IsSuccess)
            {
                await _userManager.DeleteAsync(user);
                return doctorPicResult.Errors.ToList();
            }
            var doctorPicUrl = doctorPicResult.Value;

            var syndicateResult = await UploadImageMandatoryAsync(registerDTO.SyndicateCardFile, "doctors");
            if (!syndicateResult.IsSuccess)
            {
                await _userManager.DeleteAsync(user);
                return syndicateResult.Errors.ToList();
            }
            var syndicateUrl = syndicateResult.Value;

            var doctorProfile = _mapper.Map<DoctorProfile>(registerDTO);
            doctorProfile.UserId = user.Id;
            doctorProfile.DoctorPictureUrl = doctorPicUrl!;
            doctorProfile.SyndicateCardUrl = syndicateUrl!;

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

            return new UserDTO(user.Id, user.Email!, doctorProfile.DisplayName, "Doctor", accessToken, refreshToken.Token, refreshToken.ExpiresOn);
        }

        public async Task<Result<UserDTO>> RegisterPatientAsync(RegisterPatientDTO patientDTO)
        {

            var validationError = await ValidateEmailAndPhoneAsync(patientDTO.Email, patientDTO.PhoneNumber);

            if (validationError != null)
                return validationError;

            var user = _mapper.Map<ApplicationUser>(patientDTO);

            var identityResult = await _userManager.CreateAsync(user, patientDTO.Password);
            if (!identityResult.Succeeded)
                return identityResult.Errors.Select(e => Error.Validation(e.Code, e.Description)).ToList();

            await _userManager.AddToRoleAsync(user, "Patient");

            var patientProfile = _mapper.Map<PatientProfile>(patientDTO);
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

            return new UserDTO(user.Id, user.Email!, patientProfile.DisplayName, "Patient", accessToken, refreshToken.Token, refreshToken.ExpiresOn);
        }

        public async Task<Result<UserDTO>> LoginAsync(LoginDTO loginDTO)
        {
            var user = await _userManager.FindByEmailAsync(loginDTO.Email);
            if (user == null)
                return Error.InvalidCredentials("User.InvalidEmail");

            var isPasswordValid = await _userManager.CheckPasswordAsync(user, loginDTO.Password);
            if (!isPasswordValid)
                return Error.InvalidCredentials("User.InvalidPassword");

            var roles = await _userManager.GetRolesAsync(user);
            var role = roles.First();

            var refreshToken = await GenerateRefreshTokenAsync(user);
            var accessToken = await CreateTokenAsync(user);

            return new UserDTO(user.Id, user.Email!, $"{user.FirstName} {user.LastName} ", role, accessToken, refreshToken.Token, refreshToken.ExpiresOn);
        }

        public async Task<Result<UserDTO>> RefreshTokenAsync(string refreshToken)
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

            return new UserDTO(user.Id, user.Email!, displayName, role, accessToken, newRefreshToken.Token, newRefreshToken.ExpiresOn);
        }

        public async Task<Result> ForgetPasswordAsync(ForgotPasswordDTO forgotPasswordDTO)
        {
            var user = await _userManager.FindByEmailAsync(forgotPasswordDTO.Email);
            if (user is null)
                return Result.Fail(Error.Unauthorized("Auth.UserNotFound", "No account found with this email."));

            var result = await _otpService.SendOtpAsync(forgotPasswordDTO.Email, OtpPurposeDTO.ForgotPassword);

            if (result is null)
                return Result.Fail(Error.Failure("Failed.ForgetPassword", "Error In Sending Email Try Again"));

            return Result.Ok();
        }

        public async Task<Result> VerifyOtpAsync(VerifyOtpDTO verifyOtpDTO)
        {
            var user = await _userManager.FindByEmailAsync(verifyOtpDTO.Email);
            if (user is null)
                return Result.Fail(Error.Unauthorized("Auth.UserNotFound", "No account found with this email."));

            var result = await _otpService.VerifyOtpAsync(verifyOtpDTO.Email, verifyOtpDTO.OtpCode, OtpPurposeDTO.ForgotPassword);
            if (result.IsFailure)
                return Result.Fail(Error.Failure("Failed.VerifyOtp", "OTP is not correct, Try again"));

            var markedVerify = await _otpService.MarkOtpVerifiedAsync(verifyOtpDTO.Email, OtpPurposeDTO.ForgotPassword);
            if (markedVerify.IsFailure)
                return Result.Fail(Error.Failure("Failed.markedVerify", "OTP is not correct, Try again"));

            return Result.Ok();
        }

        public async Task<Result> ResetPasswordAsync(ResetPasswordOtpDTO resetPasswordOtpDTO)
        {
            var user = await _userManager.FindByEmailAsync(resetPasswordOtpDTO.Email);
            if (user is null)
                return Result.Fail(Error.Unauthorized("Auth.UserNotFound", "No account found with this email."));

            var isVerifiedResult = await _otpService.IsOtpVerifiedAsync(resetPasswordOtpDTO.Email, OtpPurposeDTO.ForgotPassword);

            if (isVerifiedResult.IsFailure || !isVerifiedResult.Value)
                return Result.Fail(Error.Failure("Auth.OtpNotVerified", "OTP not verified."));

            var resetToken = await _userManager.GeneratePasswordResetTokenAsync(user);

            var result = await _userManager.ResetPasswordAsync(user, resetToken, resetPasswordOtpDTO.Password);

            if (!result.Succeeded)
                return Result.Fail(Error.Failure("Auth.PasswordResetFailed", "Password reset failed."));

            await _otpService.DeleteOtpAsync(resetPasswordOtpDTO.Email, OtpPurposeDTO.ForgotPassword);

            return Result.Ok();
        }

        public async Task<Result> ChangePasswordAsync(string userId, ChangePasswordDTO changePasswordDTO)
        {
            if (changePasswordDTO.Password != changePasswordDTO.ConfirmPassword)
                return Result<string>.Fail(Error.Validation("User.PasswordMismatch", "Passwords do not match."));

            var user = await _userManager.FindByIdAsync(userId);
            if (user is null)
                return Result<string>.Fail(Error.NotFound("User.NotFound", "User not found."));

            var removePassword = await _userManager.RemovePasswordAsync(user);
            if (!removePassword.Succeeded)
                return Result<string>.Fail(Error.Failure("User.PasswordRemoveFailed", "Failed to update password."));

            var addResult = await _userManager.AddPasswordAsync(user, changePasswordDTO.Password);
            if (!addResult.Succeeded)
                return Result<string>.Fail(Error.Failure("User.PasswordChangeFailed", "Failed to update password."));

            return Result.Ok();
        }

        public async Task<Result> UpdateDoctorProfileAsync(string userId, UpdateDoctorProfileDTO updateDTO)
        {
            var userResult = await GetAndUpdateDoctorAsync(userId, updateDTO);
            if (userResult.IsFailure)
                return userResult;

            var user = userResult.Value!;

            var userUpdateResult = await _userManager.UpdateAsync(user);
            if (!userUpdateResult.Succeeded)
            {
                var error = userUpdateResult.Errors.First();
                return Result.Fail(Error.Failure(error.Code, error.Description));
            }

            var doctorResult = await GetAndUpdateDoctorProfileAsync(userId, updateDTO);
            if (doctorResult.IsFailure)
                return doctorResult;

            var doctorProfile = doctorResult.Value!;

            try
            {
                _unitOfWork.GetRepository<DoctorProfile, int>().Update(doctorProfile);
                await _unitOfWork.SaveChanges();
            }
            catch (Exception ex)
            {
                return Result.Fail(Error.Failure("DoctorProfile.UpdateFailed", ex.Message));
            }

            return Result.Ok();
        }

        public async Task<Result> UpdatePatientProfileAsync(string userId, UpdatePatientProfileDTO request)
        {
            var result = await GetAndUpdatePatientProfileAsync(userId, request);
            if (!result.IsSuccess)
                return result;

            try
            {
                await _unitOfWork.SaveChanges();
            }
            catch (Exception ex)
            {
                return Result.Fail(Error.Failure("PatientProfile.UpdateFailed", ex.Message));
            }

            return Result.Ok();
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

        private async Task<Result<DoctorProfile>> GetAndUpdateDoctorProfileAsync(string userId, UpdateDoctorProfileDTO dto)
        {
            var spec = new DoctorByIdSpecification(userId);
            var doctorProfile = await _unitOfWork
                .GetRepository<DoctorProfile, int>()
                .GetByIdAsync(spec);

            if (doctorProfile is null)
                return Error.NotFound("DoctorProfile.NotFound", "Doctor profile not found.");

            doctorProfile.Specialization = dto.Specialization ?? doctorProfile.Specialization;
            doctorProfile.Bio = dto.Bio ?? doctorProfile.Bio;
            doctorProfile.ClinicLocation = dto.ClinicLocation ?? doctorProfile.ClinicLocation;
            doctorProfile.PhoneClinc = dto.PhoneClinic ?? doctorProfile.PhoneClinc;

            if (dto.YearsOfExperience.HasValue)
                doctorProfile.YearsOfExperience = dto.YearsOfExperience.Value;

            if (dto.Address is not null)
                doctorProfile.Address = _mapper.Map<Address>(dto.Address);

            if (dto.Gender.HasValue)
                doctorProfile.Gender = (Gender)dto.Gender.Value;

            if (dto.DoctorPictureFile is not null)
            {
                if (!string.IsNullOrEmpty(doctorProfile.DoctorPictureUrl))
                    await _attachmentService.DeleteImageAsync(doctorProfile.DoctorPictureUrl);

                var pictureResult = await UploadImageMandatoryAsync(dto.DoctorPictureFile, "doctors");
                if (!pictureResult.IsSuccess)
                    return Error.Validation("PictureUpdate.Failed", "Picture update failed");

                doctorProfile.DoctorPictureUrl = pictureResult.Value!;
            }

            if (dto.SyndicateCardFile is not null)
            {
                if (!string.IsNullOrEmpty(doctorProfile.SyndicateCardUrl))
                    await _attachmentService.DeleteImageAsync(doctorProfile.SyndicateCardUrl);

                var syndicateResult = await UploadImageMandatoryAsync(dto.SyndicateCardFile, "doctors");
                if (!syndicateResult.IsSuccess)
                    return Error.Validation("SyndicateCardUpdate.Failed", "Syndicate card update failed");

                doctorProfile.SyndicateCardUrl = syndicateResult.Value!;
            }

            return Result<DoctorProfile>.Ok(doctorProfile);
        }

        private async Task<Result<ApplicationUser>> GetAndUpdateDoctorAsync(string userId, UpdateDoctorProfileDTO dto)
        {
            var user = await _userManager.FindByIdAsync(userId);
            if (user is null)
                return Error.NotFound("User.NotFound", "User not found.");

            user.FirstName = dto.FirstName ?? user.FirstName;
            user.LastName = dto.LastName ?? user.LastName;
            user.PhoneNumber = dto.PhoneNumber ?? user.PhoneNumber;

            var updateResult = await _userManager.UpdateAsync(user);
            if (!updateResult.Succeeded)
            {
                var error = updateResult.Errors.First();
                return Error.Failure(error.Code, error.Description);
            }

            return Result<ApplicationUser>.Ok(user);
        }

        private async Task<Result> GetAndUpdatePatientProfileAsync(string userId, UpdatePatientProfileDTO request)
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
                expires: DateTime.UtcNow.AddHours(1),
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

        #endregion

    }
}
