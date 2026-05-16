using Health.Domain.Entities.AppointmentModule;
using Health.Domain.Entities.ConsultationModule.Enums;
using Health.Domain.Entities.DoctorModule;
using Health.Domain.Entities.PatientModule;
using System;
using System.Collections.Generic;
using System.Numerics;
using System.Text;

namespace Health.Domain.Entities.ConsultationModule
{
    public class Consultation : BaseEntity<int>
    {
        public int AppointmentId { get; set; }
        public Appointment Appointment { get; set; } = default!;
        public int DoctorId { get; set; }
        public DoctorProfile Doctor { get; set; } = default!;
        public int PatientId { get; set; }
        public PatientProfile Patient { get; set; } = default!;
        public ConsultationStatus Status { get; set; }
        public ConsultationType Type { get; set; }
        public string RoomId { get; set; } = $"consultation-{Guid.NewGuid()}";
        public DateTime ScheduledAt { get; set; }
        public DateTime? StartedAt { get; set; }
        public DateTime? EndedAt { get; set; }
        public string Symptoms { get; set; } = default!;
        public string Diagnosis { get; set; } = default!;
        public string? Notes { get; set; }
        public ICollection<Prescription> Prescriptions { get; set; } = new List<Prescription>();
    }
}
