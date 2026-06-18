using System;
using System.Collections.Generic;
using System.Text;

namespace Health.Shared.DTOs.DoctorDTOs
{
    public sealed record DoctorDashboardResponse(
        DoctorProfileSummaryResponse? Profile,
        DashboardCardsResponse? Cards,
        DashboardTablesResponse? Tables
    );
}
