using Health.Services.Abstraction.DoctorModulesAbstractions;
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
        public async Task<ActionResult<IEnumerable<DoctorDTO>>> GetAllDoctors()
        {
            var result = await _doctorService.GetAllDoctorsAsync();

            return HandleResult<IEnumerable<DoctorDTO>>(result);
        }


        // Get Doctors By Id
        [HttpGet("{id}")]
        // GET: baseUrl/api/Doctors/2
        public async Task<ActionResult<DoctorDTO>> GetDoctorById(int id)
        {

            var result = await _doctorService.GetDoctorByIdAsync(id);

            return HandleResult<DoctorDTO>(result);
        }


    }
}
