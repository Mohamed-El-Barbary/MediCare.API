using Health.Domain.Entities.AppointmentModule;
using Health.Domain.Entities.DoctorModule;
using Health.Domain.Entities.PatientModule;
using System;
using System.Collections.Generic;
using System.Text;

namespace Health.Domain.Entities.ReviewModule
{
    public class Review : BaseEntity<int>
    {
        public int doctorId { get; set; }
        public DoctorProfile doctor { get; set; } = default!;
        public int PatientId { get; set; }
        public PatientProfile Patient { get; set; } = default!;
        public int AppointmentId { get; set; }
        public Appointment Appointment { get; set; } = default!;
        public int Rating { get; set; }
        public string? Comment { get; set; }
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
        public DateTime? UpdateAt { get; set; }
    }
}
