using System;
using System.Collections.Generic;
using System.Text;

namespace Health.Shared.DTOs.PatientDashboardDTOs
{
    public sealed record PatientDashboardResponce
    (
        PatientProfileResponce profileData,
        UpcomingAppointments UpcomingAppointments,
        RecentPrescriptions RecentPrescriptions
    );
}
