using System;
using System.Collections.Generic;
using System.Text;

namespace Health.Shared.DTOs.DoctorDTOs
{
    public sealed record DashboardTablesResponse(
        IReadOnlyList<TodayAppointmentItemResponse> TodayAppointments,
        IReadOnlyList<NewRequestItemResponse> NewRequests,
        IReadOnlyList<RecentPatientItemResponse> RecentPatients
    );
}
