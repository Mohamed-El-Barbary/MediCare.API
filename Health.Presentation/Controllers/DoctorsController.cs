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

        [Authorize(Roles = "Doctor")]
        [HttpGet("Dashboard")]
        public async Task<ActionResult<DoctorDashboardResponse>> GetDashboard()
        {
            var id = GetUserId();
            var result = await _doctorService.GetDashboardAsync(id);
            return HandleResult(result);
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

        [Authorize(Roles = "Doctor")]
        [HttpGet("schedule")]
        public async Task<ActionResult<IEnumerable<DoctorSceduleToReturn>>> GetAllDoctorScedule()
        {
            var userId = GetUserId();
            var result = await _doctorService.GetAllDoctorScheduleAsync(userId);
            return HandleResult(result);
        }

        [Authorize(Roles = "Doctor")]
        [HttpDelete("{scheduleId}")]
        public async Task<ActionResult> DeleteSpacificSchdeuleFroDoctorProfile([FromRoute] int scheduleId)
        {
            string userId = GetUserId();
            var result = await _doctorService.DeleteScheduleAsync(scheduleId, userId);
            return HandleResult(result , "Schedule deleted successfully");
        }

        [Authorize(Roles = "Doctor")]
        [HttpPut("schedule/{scheduleId}")]
        public async Task<IActionResult> UpdateSchedule([FromRoute] int scheduleId,[FromBody] DoctorScheduleDTO dto)
        {
            string userDoctorId = GetUserId();
            var result = await _doctorService.UpdateScheduleAsync(userDoctorId, scheduleId, dto);
            return HandleResult(result , "Schedule Updated Successfully");
        }

        [Authorize(Roles = "Doctor")]
        [HttpPost("{scheduleId}/generate-slots")]
        public async Task<IActionResult> GenerateSlots(int scheduleId , GeneratedSlotsRequestDto requestDTO)
        {
            var result = await _doctorService.GenerateSlotsBySchedule(scheduleId, requestDTO);

            return HandleResult(result, "Doctor Slots has been established successfully.");
        }

        [Authorize(Roles = "Patient,Doctor")]
        [HttpGet("generate-slots")]
        public async Task<ActionResult<IEnumerable<GeneratedSlotsDTO>>> GetDoctorSlots(DateTime? Date)
        {
            string userDoctorId = GetUserId();
            var result = await _doctorService.GetDoctorSlots(userDoctorId, Date);

            return HandleResult(result);
        }

    }
}
