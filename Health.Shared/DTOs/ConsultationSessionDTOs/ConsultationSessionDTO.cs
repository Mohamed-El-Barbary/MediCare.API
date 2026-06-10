using System;
using System.Collections.Generic;
using System.Text;

namespace Health.Shared.DTOs.ConsultationSessionDTOs
{
    public class ConsultationSessionDTO
    {
        public int ConsultationId { get; set; }
        public string RoomId { get; set; } = default!;
        public int DoctorId { get; set; }
        public int PatientId { get; set; }
        public string? DoctorName { get; set; }
        public string? PatientName { get; set; }
        public string? Status { get; set; }
        public DateTime ScheduledAt { get; set; }
        public DateTime? StartedAt { get; set; }
        public DateTime? EndedAt { get; set; }
    }
}
