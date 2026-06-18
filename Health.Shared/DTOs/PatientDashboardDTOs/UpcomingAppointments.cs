namespace Health.Shared.DTOs.PatientDashboardDTOs
{
    public sealed record UpcomingAppointments
    (
       IEnumerable<UpcommingPatientAppointments> UpcommingAppointments
    );
}