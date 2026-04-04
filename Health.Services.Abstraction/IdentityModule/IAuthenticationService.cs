using Health.Shared.CommonResponses;
using Health.Shared.DTOs.IdentityDTOs;
using System;
using System.Collections.Generic;
using System.Text;

namespace Health.Services.Abstraction.IdentityModuleAbstraction
{
    public interface IAuthenticationService
    {
        Task<Result<UserDTO>> RegisterDoctorAsync(RegisterDoctorDTO registerDTO);
        Task<Result<UserDTO>> RegisterPatientAsync(RegisterPatientDTO patientDTO);
        Task<Result<UserDTO>> LoginAsync(LoginDTO loginDTO);
        Task<Result<UserDTO>> RefreshTokenAsync(string refreshToken);
        Task<Result> ForgetPasswordAsync(ForgotPasswordDTO forgotPasswordDTO);
        Task<Result> VerifyOtpAsync(VerifyOtpDTO verifyOtpDTO);
        Task<Result> ResetPasswordAsync(ResetPasswordOtpDTO resetPasswordOtpDTO);
        Task<Result> ChangePasswordAsync(string userId, ChangePasswordDTO changePasswordDTO);
        Task<Result> UpdateDoctorProfileAsync(string userId, UpdateDoctorProfileDTO request);
        Task<Result> UpdatePatientProfileAsync(string userId, UpdatePatientProfileDTO request);
    }
}
