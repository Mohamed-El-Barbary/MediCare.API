using System;
using System.Collections.Generic;
using System.Text;

namespace Health.Shared.DTOs.DoctorDTOs
{
    public sealed record NewRequestItemResponse(
        int AppointmentId,
        string PatientName,
        string Type,
        DateTime RequestedAt,
        string Status
    );
}
