using System;
using System.Collections.Generic;
using System.Text;

namespace Health.Shared.DTOs.ConsultationDTOs
{
    public class ConsultationDTO
    {
        public int Id { get; set; }
        public int AppointmentId { get; set; }
        public int DoctorId { get; set; }
        public string DoctorName { get; set; } = default!;
        public int PatientId { get; set; }
        public string PatientName { get; set; } = default!;
        public string Status { get; set; } = default!;
        public string Type { get; set; } = default!;
        public string RoomId { get; set; } = default!;
        public DateTime ScheduledAt { get; set; } = DateTime.UtcNow;
        public DateTime? StartedAt { get; set; }
        public DateTime? EndedAt { get; set; }
        public string? Symptoms { get; set; }
        public string? Diagnosis { get; set; }
        public string? Notes { get; set; }
        public ICollection<PrescriptionDTO> Prescriptions { get; set; }
            = new List<PrescriptionDTO>();

    }
}
