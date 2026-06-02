using System;
using System.Collections.Generic;
using System.Text;

namespace Health.Shared.DTOs.ConsultationDTOs
{
    public class ConsultationSummaryDTO
    {
        public int Id { get; set; }
        public int DoctorId { get; set; }
        public string DoctorName { get; set; } = default!;
        public int PatientId { get; set; }
        public string PatientName { get; set; } = default!;
        public string Status { get; set; } = default!;
        public string Type { get; set; } = default!;
        public DateTime ScheduledAt { get; set; }
        public DateTime? StartedAt { get; set; }
        public DateTime? EndedAt { get; set; }
    }
}
