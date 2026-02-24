using Health.Services.Abstraction.DoctorModulesAbstractions;
using Health.Shared;
using Health.Shared.CommonResponses;
using Health.Shared.DTOs.DoctorDTOs;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
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

    }
}
