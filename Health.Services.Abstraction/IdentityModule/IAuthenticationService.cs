using Health.Shared.CommonResponses;
using Health.Shared.DTOs.IdentityDTOs;
using Health.Shared.DTOs.IdentityDTOs.Requests;
using Health.Shared.DTOs.IdentityDTOs.Responses;
using System;
using System.Collections.Generic;
using System.Text;

namespace Health.Services.Abstraction.IdentityModuleAbstraction
{
    public interface IAuthenticationService
    {
        Task<Result<RegisterResponse>> RegisterDoctorAsync(RegisterDoctorRequest request);
        Task<Result<RegisterResponse>> RegisterPatientAsync(RegisterPatientRequest request);
        Task<Result<LoginResponse>> LoginAsync(LoginRequest request);
        Task<Result<TokenResponse>> RefreshTokenAsync(string refreshToken);
        Task<Result<CommandResponse>> ForgetPasswordAsync(ForgotPasswordRequest request);
        Task<Result<CommandResponse>> VerifyOtpAsync(VerifyOtpRequest request);
        Task<Result<CommandResponse>> ResendOtpAsync(ResendOtpRequest request);
        Task<Result<CommandResponse>> ResetPasswordAsync(ResetPasswordRequest request);
        Task<Result<CommandResponse>> ChangePasswordAsync(string userId, ChangePasswordRequest request);
        Task<Result<DoctorProfileResponse>> UpdateDoctorProfileAsync(string userId, UpdateDoctorProfileRequest request);
        Task<Result<CommandResponse>> UpdatePatientProfileAsync(string userId, UpdatePatientProfileRequest request);
    }
}
