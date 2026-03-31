using Health.Shared.CommonResponses;
using Health.Shared.DTOs.IdentityDTOs;
using System;
using System.Collections.Generic;
using System.Text;

namespace Health.Services.Abstraction
{
    public interface IAuthenticationService
    {
        Task<Result<UserDTO>> RegisterDoctorAsync(RegisterDoctorDTO registerDTO);
        Task<Result<UserDTO>> RegisterPatientAsync(RegisterPatientDTO patientDTO);
        Task<Result<UserDTO>> LoginAsync(LoginDTO loginDTO);
        Task<Result<UserDTO>> RefreshTokenAsync(string refreshToken);
    }
}
