using Health.Presentation.Attributes;
using Health.Services.Abstraction.PatientServiceAbstraction;
using Health.Shared.CommonResponses;
using Health.Shared.DTOs.PatientDashboardDTOs;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Text;

namespace Health.Presentation.Controllers
{
    public class PatientController : ApiBaseController
    {
        private readonly IPatientService _patientService;

        public PatientController(IPatientService patientService)
        {
            _patientService = patientService;
        }


        [HttpGet("Dashboard")]
        [Authorize(Roles = "Patient")]
        [RedisCache(5)]
        public async Task<ActionResult<PatientDashboardResponce>> PatientDashboard()
        {
            var PatientId = GetUserId();
            var result = await _patientService.GetPatientDashboard(PatientId);
            return HandleResult(result);
        }


    }
}
