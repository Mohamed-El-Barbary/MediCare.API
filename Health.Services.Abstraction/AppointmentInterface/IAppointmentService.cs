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
        Task<Result<AppointmentStatisticsResponse>> GetAppointmentStatisticsAsync(string userId);
        Task<Result<DoctorAppointmentDTO>> BookAppointmentAsync(CreateAppointmentDTO createAppointmentDTO, string patientUserId);
        Task<PaginatedResult<PatientAppointmentDTO>> GetPatientAppointment(string PatientUserId, AppointmentSpecParams specParams);
        Task<PaginatedResult<DoctorAppointmentDTO>> GetDoctorAppointment(string DoctorUserId, AppointmentSpecParams specParams);
        Task<Result<DoctorAppointmentDTO>> GetDoctorAppointmentForSpacificPatient(int AppointmentId);
        Task<Result<PatientAppointmentDTO>> GetPatientAppointmentForSpacificDoctor(int AppointmentId);
        Task<Result> CancelAppointmentAsync(int AppointmentId , string userId , string role);
        Task<Result> ConfirmAppointment(int appointmentId, string userId);
        Task<Result> CompleteAppointment(int appointmentId, string userId);

    }
}
