using Health.Services.Abstraction.IdentityModuleAbstraction;
using Health.Shared.CommonResponses;
using Health.Shared.DTOs.IdentityDTOs;
using Health.Shared.DTOs.IdentityDTOs.Requests;
using Health.Shared.DTOs.IdentityDTOs.Responses;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Text;
using static System.Net.WebRequestMethods;

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
        public async Task<ActionResult<RegisterResponse>> RegisterDoctor(RegisterDoctorRequest request)
        {
            var result = await _authenticationService.RegisterDoctorAsync(request);

            if (!result.IsSuccess)
                return HandleResult(result);

            SetRefreshTokenCookie(result.Value.Token.RefreshToken, result.Value.Token.ExpiresAt);

            return Ok(result.Value);
        }

        [HttpPost("register-patient")]
        public async Task<ActionResult<RegisterResponse>> RegisterPatient(RegisterPatientRequest request)
        {
            var result = await _authenticationService.RegisterPatientAsync(request);

            if (!result.IsSuccess)
                return HandleResult(result);

            SetRefreshTokenCookie(result.Value.Token.RefreshToken, result.Value.Token.ExpiresAt);

            return Ok(result.Value);
        }

        [HttpPost("login")]
        public async Task<ActionResult<LoginResponse>> Login(LoginRequest request)
        {
            var result = await _authenticationService.LoginAsync(request);

            if (!result.IsSuccess)
                return HandleResult(result);

            SetRefreshTokenCookie(result.Value.RefreshToken, result.Value.ExpiresAt);

            return Ok(result.Value);
        }

        [HttpPost("refresh-token")]
        public async Task<ActionResult<TokenResponse>> RefreshToken()
        {
            var oldToken = Request.Cookies["refreshToken"];
            var result = await _authenticationService.RefreshTokenAsync(oldToken!);

            if (!result.IsSuccess) return HandleResult(result);

            SetRefreshTokenCookie(result.Value.RefreshToken, result.Value.ExpiresAt);

            return Ok(result.Value);
        }

        [HttpPost("forget-password")]
        public async Task<ActionResult<CommandResponse>> ForgetPassord(ForgotPasswordRequest request)
        {
            var result = await _authenticationService.ForgetPasswordAsync(request);
            return HandleResult(result);
        }

        [HttpPost("verfiy-otp")]
        public async Task<ActionResult<CommandResponse>> VerifyOtp(VerifyOtpRequest request)
        {
            var result = await _authenticationService.VerifyOtpAsync(request);
            return HandleResult(result);
        }

        [HttpPost("resend-otp")]
        public async Task<ActionResult<CommandResponse>> ResendOtp(ResendOtpRequest request)
        {
            var result = await _authenticationService.ResendOtpAsync(request);
            return HandleResult(result);
        }

        [HttpPost("reset-password")]
        public async Task<ActionResult<CommandResponse>> ResetPassword(ResetPasswordRequest request)
        {
            var result = await _authenticationService.ResetPasswordAsync(request);
            return HandleResult(result);
        }

        [HttpPost("change-password")]
        public async Task<ActionResult<CommandResponse>> ChangePassword(ChangePasswordRequest request)
        {
            var userId = GetUserId();
            var result = await _authenticationService.ChangePasswordAsync(userId, request);
            return HandleResult(result);
        }

        [HttpPut("doctor")]
        public async Task<ActionResult<DoctorProfileResponse>> UpdateDoctorProfile(UpdateDoctorProfileRequest updateDoctor)
        {
            var userId = GetUserId();
            var result = await _authenticationService.UpdateDoctorProfileAsync(userId, updateDoctor);
            return HandleResult(result);
        }

        [HttpPut("patient")]
        public async Task<ActionResult<CommandResponse>> UpdatePatientProfile(UpdatePatientProfileRequest request)
        {
            var userId = GetUserId();
            var result = await _authenticationService.UpdatePatientProfileAsync(userId, request);
            return HandleResult(result);
        }

    }
}
