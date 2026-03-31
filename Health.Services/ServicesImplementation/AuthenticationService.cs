using AutoMapper;
using Health.Domain.Contracts;
using Health.Domain.Entities;
using Health.Domain.Entities.DoctorModule;
using Health.Domain.Entities.IdentityModule;
using Health.Domain.Entities.PatientModule;
using Health.Services.Abstraction;
using Health.Shared.CommonResponses;
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

namespace Health.Services.ServicesImplementation
{
    public class AuthenticationService : IAuthenticationService
    {

        private readonly UserManager<ApplicationUser> _userManager;
        private readonly IConfiguration _configuration;
        private readonly IUnitOfWork _unitOfWork;
        private readonly IMapper _mapper;
        private readonly IAttachmentService _attachmentService;

        public AuthenticationService(
            UserManager<ApplicationUser> userManager,
            IConfiguration configuration,
            IUnitOfWork unitOfWork,
            IMapper mapper,
            IAttachmentService attachmentService)
        {
            _userManager = userManager;
            _configuration = configuration;
            _unitOfWork = unitOfWork;
            _mapper = mapper;
            _attachmentService = attachmentService;
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

        private async Task<string> CreateTokenAsync(ApplicationUser user)
        {
            // Token [Issuer, Audience, Claims, Expires, Signing Credentials]

            var claims = new List<Claim>
            {
                new Claim("userId", user.Id),
                new Claim("displayName", $"{user.FirstName!} {user.LastName!}"),
                new Claim(JwtRegisteredClaimNames.Email, user.Email!),
                new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString()),
            };

            var roles = await _userManager.GetRolesAsync(user);

            foreach (var role in roles)
                claims.Add(new Claim("role", role));

            var secretKey = _configuration["JWTOptions:SecretKey"];
            var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(secretKey!));
            var cred = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

            var token = new JwtSecurityToken(
                issuer: _configuration["JWTOptions:SecretKey"],
                audience: _configuration["JWTOptions:SecretKey"],
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
