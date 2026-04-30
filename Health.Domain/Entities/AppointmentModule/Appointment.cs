using Health.Domain.Entities.DoctorModule;
using Health.Domain.Entities.PatientModule;
using System;
using System.Collections.Generic;
using System.Text;

namespace Health.Domain.Entities.AppointmentModule
{
    public class Appointment : BaseEntity<int>
    {
        // Foreign key
        public int DoctorProfileId { get; set; }
        public int PatientProfileId { get; set; }
        public int DoctorGeneratedSlotsId { get; set; }
        // Navigation Property
        public DoctorProfile DoctorProfile { get; set; } = default!;
        public PatientProfile PatientProfile { get; set; } = default!;
        public DoctorGeneratedSlots DoctorGeneratedSlots { get; set; } = default!;
        // Business Property
        public AppointmentStatus Status { get; set; }
        public AppointmentType AppointmentType { get; set; }
        public DateTime CreatedAt { get; set; }

    }
}
