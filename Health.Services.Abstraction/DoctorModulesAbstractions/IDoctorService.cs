using Health.Shared.CommonResponses;
using Health.Shared.DTOs.DoctorDTOs;
using System;
using System.Collections.Generic;
using System.Text;

namespace Health.Services.Abstraction.DoctorModulesAbstractions
{
    public interface IDoctorService
    {

        Task<Result<IEnumerable<DoctorDTO>>> GetAllDoctorsAsync();
        Task<Result<DoctorDTO>> GetDoctorByIdAsync(int id);

    }
}
