using System;
using System.Collections.Generic;
using System.Text;

namespace Health.Shared.DTOs.AppointmentDTOs
{
    public record AppointmentStatisticsResponse(
        int TotalAppointments,
        int PendingAppointments,
        int ConfirmedAppointments,
        int CompletedAppointments,
        int CancelledAppointments
    );
}
