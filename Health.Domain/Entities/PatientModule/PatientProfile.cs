using Health.Domain.Entities.AppointmentModule;
using Health.Domain.Entities.DoctorModule;
using Health.Domain.Entities.PatientModule.enums;
using Health.Domain.Entities.ReviewModule;
using System;
using System.Collections.Generic;
using System.Text;

namespace Health.Domain.Entities.PatientModule
{
    public class PatientProfile : BaseEntity<int>
    {
        public string UserId { get; set; } = null!;
        public string DisplayName { get; set; } = null!;
        public Gender Gender { get; set; }
        public DateTime DateOfBirth { get; set; }
        public DateTime JoinDate { get; set; } = DateTime.UtcNow;
        public Address Address { get; set; } = null!;
        public BloodType? BloodType { get; set; }
        public ICollection<PatientChronicDisease> PatientChronicDiseases { get; set; } = new List<PatientChronicDisease>();
        public ICollection<Appointment> Appointments { get; set; } = new List<Appointment>();
        public EmergencyContact? EmergencyContact { get; set; }
        public ICollection<Review> Reviews { get; set; } = new List<Review>();
    }
}
