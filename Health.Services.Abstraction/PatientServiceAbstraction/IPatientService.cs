using Health.Shared.CommonResponses;
using Health.Shared.DTOs.PatientDashboardDTOs;
using System;
using System.Collections.Generic;
using System.Text;

namespace Health.Services.Abstraction.PatientServiceAbstraction
{
    public interface IPatientService
    {
        Task<Result<PatientDashboardResponce>> GetPatientDashboard(string patientId);
    }
}
