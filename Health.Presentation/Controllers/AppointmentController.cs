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
        public async Task<IActionResult> Book([FromBody] CreateAppointmentDTO dto)
        {
            string patientUserId = GetUserId();
            var result = await _appointmentService.BookAppointmentAsync(dto, patientUserId);
            return HandleResult(result , "Patient Appointment has been established successfully.");
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

    }
}
