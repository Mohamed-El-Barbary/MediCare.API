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

        Task<Result> AddScheduleAsync(int doctorId , DoctorScheduleDTO DoctorScheduleDTOs);
        Task<PaginatedResult<DoctorDTO>> GetAllDoctorsAsync(DoctorSpecParams QueryParams);
        Task<Result<DoctorDTO>> GetDoctorByIdAsync(int id);
        Task<Result<IEnumerable<DoctorScheduleDTO>>> GetAllDoctorScheduleAsync(int doctorId);
    }
}
