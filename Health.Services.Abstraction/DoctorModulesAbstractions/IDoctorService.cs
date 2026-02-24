using Health.Shared;
using Health.Shared.CommonResponses;
using Health.Shared.DTOs.DoctorDTOs;
using System;
using System.Collections.Generic;
using System.Text;

namespace Health.Services.Abstraction.DoctorModulesAbstractions
{
    public interface IDoctorService
    {

        Task<PaginatedResult<DoctorDTO>> GetAllDoctorsAsync(DoctorSpecParams QueryParams);
        Task<Result<DoctorDTO>> GetDoctorByIdAsync(int id);

    }
}
