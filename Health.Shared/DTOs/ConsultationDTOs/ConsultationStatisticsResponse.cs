using System;
using System.Collections.Generic;
using System.Text;

namespace Health.Shared.DTOs.ConsultationDTOs
{
    public sealed record ConsultationStatisticsDTO
   (
       int TotalConsultations,
       int ScheduledConsultations,
       int InProgressConsultations,
       int CompletedConsultations,
       int CancelledConsultations,
       int MissedConsultations
   );
}
