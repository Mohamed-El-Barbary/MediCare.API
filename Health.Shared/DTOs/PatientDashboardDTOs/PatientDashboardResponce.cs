using Health.Shared.DTOs.ConsultationDTOs;
using System;
using System.Collections.Generic;
using System.Text;

namespace Health.Shared.DTOs.PatientDashboardDTOs
{
    public sealed record PatientDashboardResponce
    (
        PatientProfileResponce profileData,
        UpcomingAppointments UpcomingAppointments,
        IEnumerable<PrescriptionDTO> recentPrescriptions
    );
}
