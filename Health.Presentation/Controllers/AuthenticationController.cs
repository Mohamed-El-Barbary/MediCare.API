using Health.Services.Abstraction.IdentityModuleAbstraction;
using Health.Shared.CommonResponses;
using Health.Shared.DTOs.IdentityDTOs;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Text;

namespace Health.Presentation.Controllers
{
    public class AuthenticationController : ApiBaseController
    {
        private readonly IAuthenticationService _authenticationService;


        public AuthenticationController(IAuthenticationService authenticationService)
        {
            _authenticationService = authenticationService;
        }

        [HttpPost("register-doctor")]
        public async Task<ActionResult<UserDTO>> RegisterDoctor([FromForm] RegisterDoctorDTO registerDto)
        {
            var result = await _authenticationService.RegisterDoctorAsync(registerDto);

            if (!result.IsSuccess)
                return HandleResult(result);

            SetRefreshTokenCookie(result.Value.RefreshToken, result.Value.RefreshTokenExpiresOn);

            return Ok(result.Value);
        }

        [HttpPost("register-patient")]
        public async Task<ActionResult<UserDTO>> RegisterPatient(RegisterPatientDTO registerPatient)
        {
            var result = await _authenticationService.RegisterPatientAsync(registerPatient);

            if (!result.IsSuccess)
                return HandleResult(result);

            SetRefreshTokenCookie(result.Value.RefreshToken, result.Value.RefreshTokenExpiresOn);

            return Ok(result.Value);
        }

        [HttpPost("login")]
        public async Task<ActionResult<UserDTO>> Login(LoginDTO loginDTO)
        {
            var result = await _authenticationService.LoginAsync(loginDTO);

            if (!result.IsSuccess)
                return HandleResult(result);

            SetRefreshTokenCookie(result.Value.RefreshToken, result.Value.RefreshTokenExpiresOn);

            return Ok(result.Value);
        }

        [HttpPost("refresh-token")]
        public async Task<ActionResult<UserDTO>> RefreshToken()
        {
            var oldToken = Request.Cookies["refreshToken"];
            var result = await _authenticationService.RefreshTokenAsync(oldToken!);

            if (!result.IsSuccess) return HandleResult(result);

            SetRefreshTokenCookie(result.Value.RefreshToken, result.Value.RefreshTokenExpiresOn);

            return Ok(result.Value);
        }

        [HttpPost("forget-password")]
        public async Task<ActionResult> ForgetPassord(ForgotPasswordDTO forgotPasswordDTO)
        {
            var result = await _authenticationService.ForgetPasswordAsync(forgotPasswordDTO);
            return HandleResult(result, "If this email exists, an OTP has been sent.");
        }

        [HttpPost("verfiy-otp")]
        public async Task<ActionResult> VerifyOtp(VerifyOtpDTO verifyOtpDTO)
        {
            var result = await _authenticationService.VerifyOtpAsync(verifyOtpDTO);
            return HandleResult(result, "OTP verified successfully.");
        }

        [HttpPost("reset-password")]
        public async Task<ActionResult> ResetPassword(ResetPasswordOtpDTO resetPasswordOtpDTO)
        {
            var result = await _authenticationService.ResetPasswordAsync(resetPasswordOtpDTO);
            return HandleResult(result, "Password has been reset successfully.");
        }

        [HttpPost("change-password")]
        public async Task<ActionResult<string>> ChangePassword(string userId, ChangePasswordDTO changePasswordDTO)
        {
            var result = await _authenticationService.ChangePasswordAsync(userId, changePasswordDTO);
            return HandleResult(result, "Password changed successfully.");
        }

        [HttpPost("update-doctor")]
        public async Task<ActionResult> UpdateDoctorProfile(string userId, UpdateDoctorProfileDTO updateDoctor)
        {
            var result = await _authenticationService.UpdateDoctorProfileAsync(userId, updateDoctor);
            return HandleResult(result, "Profile updated successfully.");
        }

        [HttpPost("update-patient")]
        public async Task<ActionResult> UpdatePatientProfile(string userId, UpdatePatientProfileDTO updatePatient)
        {
            var result = await _authenticationService.UpdatePatientProfileAsync(userId, updatePatient);
            return HandleResult(result, "Profile updated successfully.");
        }

    }
}
