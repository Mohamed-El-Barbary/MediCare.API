using Health.Services.Abstraction.AppointmentInterface;
using Health.Shared.DTOs.AppointmentDTOs;
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


        [HttpPost("{patientId}")] 
        public async Task<IActionResult> Book([FromBody] CreateAppointmentDTO dto , int patientId)
        {
            var result = await _appointmentService.BookAppointmentAsync(dto, patientId);
            return HandleResult(result , "Patient Appointment has been established successfully.");
        }


    }
}
