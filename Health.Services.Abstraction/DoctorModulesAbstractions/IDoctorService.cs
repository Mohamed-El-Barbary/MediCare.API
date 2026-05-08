using Health.Shared;
using Health.Shared.CommonResponses;
using Health.Shared.DTOs.DoctorDTOs;
using Health.Shared.DTOs.Enums;
using System;
using System.Collections.Generic;
using System.Net.NetworkInformation;
using System.Text;
using static System.Runtime.InteropServices.JavaScript.JSType;

namespace Health.Services.Abstraction.DoctorModulesAbstractions
{
    public interface IDoctorService
    {

        Task<Result> AddScheduleAsync(int doctorId , DoctorScheduleDTO DoctorScheduleDTOs);
        Task<PaginatedResult<DoctorDTO>> GetAllDoctorsAsync(DoctorSpecParams QueryParams);
        Task<Result<DoctorDTO>> GetDoctorByIdAsync(int id);
        Task<Result<IEnumerable<DoctorSceduleToReturn>>> GetAllDoctorScheduleAsync(int doctorId);
        Task<Result> DeleteScheduleAsync(int scheduleId, int doctorProfileId);

        Task<Result> UpdateScheduleAsync(int doctorProfileId, int scheduleId, DoctorScheduleDTO dto);

        Task<Result> GenerateSlotsBySchedule(int scheduleId , GeneratedSlotsRequestDto generatedSlotsRequestDto);

        Task<Result<IEnumerable<GeneratedSlotsDTO>>> GetDoctorSlots(int doctorId, DateTime? date);

    }
}
