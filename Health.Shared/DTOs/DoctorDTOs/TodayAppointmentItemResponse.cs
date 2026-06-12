using System;
using System.Collections.Generic;
using System.Text;

namespace Health.Shared.DTOs.DoctorDTOs
{
    public sealed record TodayAppointmentItemResponse(
        int AppointmentId,
        string PatientName,
        TimeOnly Time,
        string Status,
        string Type
    );
}
