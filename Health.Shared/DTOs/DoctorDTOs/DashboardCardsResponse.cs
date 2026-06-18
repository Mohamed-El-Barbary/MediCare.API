using System;
using System.Collections.Generic;
using System.Text;

namespace Health.Shared.DTOs.DoctorDTOs
{
    public sealed record DashboardCardsResponse(
        int TodayAppointmentsCount,
        int TotalPatients,
        decimal AverageRating,
        decimal TotalPaymentsAmount
    );
}
