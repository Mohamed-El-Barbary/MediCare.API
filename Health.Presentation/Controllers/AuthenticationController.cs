using Health.Services.Abstraction;
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
            var result = await _authenticationService.RegisterAsync(registerDto);

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

            if (!result.IsSuccess) return Unauthorized();

            SetRefreshTokenCookie(result.Value.RefreshToken, result.Value.RefreshTokenExpiresOn);

            return Ok(result.Value);
        }

    }
}
