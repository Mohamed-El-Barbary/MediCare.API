using Health.Shared.CommonResponses;
using Health.Shared.DTOs.AppointmentDTOs;
using System;
using System.Collections.Generic;
using System.Text;

namespace Health.Services.Abstraction.AppointmentInterface
{
    public interface IAppointmentService
    {
        Task<Result> BookAppointmentAsync(CreateAppointmentDTO createAppointmentDTO, string patientUserId);

        Task<Result<IEnumerable<PatientAppointmentDTO>>> GetPatientAppointment(string PatientUserId);
    }
}
