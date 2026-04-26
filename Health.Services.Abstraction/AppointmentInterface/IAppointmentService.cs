using Health.Shared;
using Health.Shared.CommonResponses;
using Health.Shared.DTOs.AppointmentDTOs;
using Health.Shared.ParamsForFilterationPatientAppointment;
using System;
using System.Collections.Generic;
using System.Text;

namespace Health.Services.Abstraction.AppointmentInterface
{
    public interface IAppointmentService
    {
        Task<Result> BookAppointmentAsync(CreateAppointmentDTO createAppointmentDTO, string patientUserId);

        Task<PaginatedResult<PatientAppointmentDTO>> GetPatientAppointment(string PatientUserId, AppointmentSpecParams specParams);
        Task<PaginatedResult<DoctorAppointmentDTO>> GetDoctorAppointment(string DoctorUserId, AppointmentSpecParams specParams);

        Task<Result<DoctorAppointmentDTO>> GetDoctorAppointmentForSpacificPatient(int AppointmentId);
        Task<Result<PatientAppointmentDTO>> GetPatientAppointmentForSpacificDoctor(int AppointmentId);
    }
}
