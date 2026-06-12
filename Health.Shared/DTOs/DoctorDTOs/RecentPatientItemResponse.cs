using System;
using System.Collections.Generic;
using System.Text;

namespace Health.Shared.DTOs.DoctorDTOs
{
    public sealed record RecentPatientItemResponse(
        int PatientId,
        string PatientName,
        string? Condition,
        DateTime LastAppointmentDate
    );
}
