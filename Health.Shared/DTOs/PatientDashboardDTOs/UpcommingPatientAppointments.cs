namespace Health.Shared.DTOs.PatientDashboardDTOs
{
    public sealed record UpcommingPatientAppointments
    (
        int Id,
        string DoctorName,
        string DoctorSpecialization,
        string AppointmentDate,
        TimeSpan StartTime,
        TimeSpan EndTime,
        string Status,
        string Type 
    );
}