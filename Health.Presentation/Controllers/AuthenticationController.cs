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

        [HttpPost("register")]
        public async Task<ActionResult<UserDTO>> RegisterDoctor([FromBody] RegisterDoctorDTO dto)
        {
            var result = await _authenticationService.RegisterAsync(dto);
            return HandleResult(result);
        }

    }
}
