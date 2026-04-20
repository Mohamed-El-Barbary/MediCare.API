using Health.Services.Abstraction.DoctorModulesAbstractions;
using Health.Shared;
using Health.Shared.CommonResponses;
using Health.Shared.DTOs.DoctorDTOs;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Security.Claims;
using System.Text;

namespace Health.Presentation.Controllers
{
    public class DoctorsController : ApiBaseController
    {
        private readonly IDoctorService _doctorService;

        public DoctorsController(IDoctorService doctorService)
        {
            _doctorService = doctorService;
        }

        [HttpGet]
        public async Task<ActionResult<PaginatedResult<DoctorDTO>>> GetAllDoctors([FromQuery] DoctorSpecParams specParams)
        {
            var result = await _doctorService.GetAllDoctorsAsync(specParams);

            return Ok(result);
        }

        [HttpGet("{id}")]
        public async Task<ActionResult<DoctorDTO>> GetDoctorById(int id)
        {
            var result = await _doctorService.GetDoctorByIdAsync(id);

            return HandleResult<DoctorDTO>(result);
        }

        [Authorize(Roles = "Doctor")]
        [HttpPost("schedule")]
        public async Task<IActionResult> AddSchedule([FromBody] DoctorScheduleDTO dto)
        {
            var userId = GetUserId();
            var result = await _doctorService.AddScheduleAsync(userId, dto);

            return HandleResult(result , "Doctor schedule has been established successfully.");
        }

        [HttpGet("{id}/schedule")]
        public async Task<ActionResult<IEnumerable<DoctorSceduleToReturn>>> GetAllDoctorScedule(int id)
        {
            var result = await _doctorService.GetAllDoctorScheduleAsync(id);
            return HandleResult(result);
        }

        [HttpDelete("{scheduleId}")]
        public async Task<ActionResult> DeleteSpacificSchdeuleFroDoctorProfile([FromRoute] int scheduleId , [FromQuery] int doctorProfileId)
        {
            var result = await _doctorService.DeleteScheduleAsync(scheduleId, doctorProfileId);
            return HandleResult(result , "Schedule deleted successfully");
        }


        [HttpPut("{doctorProfileId}/schedule/{scheduleId}")]
        public async Task<IActionResult> UpdateSchedule([FromRoute] int doctorProfileId,[FromRoute] int scheduleId,[FromBody] DoctorScheduleDTO dto)
        {
            var result = await _doctorService.UpdateScheduleAsync(doctorProfileId, scheduleId, dto);
            return HandleResult(result , "Schedule Updated Successfully");
        }

        [HttpPost("{scheduleId}/generate-slots")]
        public async Task<IActionResult> GenerateSlots(int scheduleId , GeneratedSlotsRequestDto requestDTO)
        {
            var result = await _doctorService.GenerateSlotsBySchedule(scheduleId, requestDTO);

            return HandleResult(result, "Doctor Slots has been established successfully.");
        }

        [HttpGet("{doctorId}/generate-slots")]
        public async Task<ActionResult<IEnumerable<GeneratedSlotsDTO>>> GetDoctorSlots(int doctorId, DateTime? Date)
        {
            var result = await _doctorService.GetDoctorSlots(doctorId, Date);

            return HandleResult(result);
        }


        #region HelperMethod

            private string GetUserId()
            {
                var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;

                if (string.IsNullOrEmpty(userId))
                    throw new UnauthorizedAccessException("Invalid token");

                return userId;
            }
    

        #endregion


    }
}
