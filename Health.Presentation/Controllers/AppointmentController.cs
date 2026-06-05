using Health.Services.Abstraction.AppointmentInterface;
using Health.Shared;
using Health.Shared.DTOs.AppointmentDTOs;
using Health.Shared.ParamsForFilterationPatientAppointment;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Text;

namespace Health.Presentation.Controllers
{
    public class AppointmentController : ApiBaseController
    {
        private readonly IAppointmentService _appointmentService;

        public AppointmentController(IAppointmentService appointmentService)
        {
            _appointmentService = appointmentService;
        }


        [Authorize(Roles = "Patient")]
        [HttpPost()] 
        public async Task<ActionResult<DoctorAppointmentDTO>> Book([FromBody] CreateAppointmentDTO dto)
        {
            string patientUserId = GetUserId();
            var result = await _appointmentService.BookAppointmentAsync(dto, patientUserId);
            return HandleResult(result);
        }

        [Authorize(Roles = "Patient")]
        [HttpGet]
        public async Task<ActionResult<PaginatedResult<PatientAppointmentDTO>>> GetAllPatientAppointments([FromQuery] AppointmentSpecParams specParams)
        {
            string patientUserId = GetUserId();
            var result = await _appointmentService.GetPatientAppointment(patientUserId , specParams);
            return Ok(result);
        }

        [Authorize(Roles = "Doctor")]
        [HttpGet("Doctor")]
        public async Task<ActionResult<PaginatedResult<DoctorAppointmentDTO>>> GetAllDoctorAppointment([FromQuery] AppointmentSpecParams specParams)
        {
            string DoctorUserId = GetUserId();
            var result = await _appointmentService.GetDoctorAppointment(DoctorUserId , specParams);
            return Ok(result);
        }

        [Authorize(Roles = "Doctor")]
        [HttpGet("Doctor/{AppointmentId}")]
        public async Task<ActionResult<DoctorAppointmentDTO>> GetDoctorAppointmentForSpacificPatient([FromRoute] int AppointmentId)
        {
            var result = await _appointmentService.GetDoctorAppointmentForSpacificPatient(AppointmentId);
            return HandleResult(result);
        }

        [Authorize(Roles = "Patient")]
        [HttpGet("Patient/{AppointmentId}")]
        public async Task<ActionResult<PatientAppointmentDTO>> GetPatientAppointmentForSpacificDoctor([FromRoute] int AppointmentId)
        {
            var result = await _appointmentService.GetPatientAppointmentForSpacificDoctor(AppointmentId);
            return HandleResult(result);
        }

        [HttpDelete("{appointmentId}")]
        [Authorize(Roles = "Patient,Doctor")]
        public async Task<IActionResult> CancelAppointment(int appointmentId)
        {
            string userId = GetUserId();
            string role = GetUserRole();
            var result = await _appointmentService.CancelAppointmentAsync(appointmentId, userId, role);
            return HandleResult(result , "Appointment cancelled successfully");
        }

        [HttpPut("{appointmentId}/Confirmed")]
        [Authorize(Roles = "Doctor")]
        public async Task<IActionResult> ConfirmeAppointment(int appointmentId)
        {
            string userId = GetUserId();
            var result = await _appointmentService.ConfirmAppointment(appointmentId , userId);
            return HandleResult(result, "Appointment Confirmed successfully");
        }
        [HttpPut("{appointmentId}/Complete")]
        [Authorize(Roles = "Doctor")]
        public async Task<IActionResult> CompleteAppointment(int appointmentId)
        {
            string userId = GetUserId();
            var result = await _appointmentService.CompleteAppointment(appointmentId , userId);
            return HandleResult(result, "Appointment Complete successfully");
        }

    }
}
