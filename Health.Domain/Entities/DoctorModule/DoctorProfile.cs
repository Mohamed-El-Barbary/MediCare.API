using Health.Domain.Entities.AppointmentModule;
using Health.Domain.Entities.IdentityModule;
using Health.Domain.Entities.ReviewModule;
using System;
using System.Collections.Generic;
using System.Text;

namespace Health.Domain.Entities.DoctorModule
{
    public class DoctorProfile : BaseEntity<int>
    {
        public string UserId { get; set; } = default!;
        public string DisplayName { get; set; } = default!;
        public string? PhoneClinc { get; set; }
        public decimal PriceConsultation { get; set; }
        public Gender Gender { get; set; }
        public DateTime JoinDate { get; set; } = DateTime.UtcNow;
        public Address Address { get; set; } = default!;
        public string Specialization { get; set; } = default!;
        public string? Bio { get; set; }
        public string ClinicLocation { get; set; } = default!;
        public string? SyndicateCardUrl { get; set; }
        public int YearsOfExperience { get; set; } = default!;
        public VerificationStatus VerificationStatus { get; set; } = VerificationStatus.PendingProfileCompletion;
        public string? DoctorPictureUrl { get; set; }
        public string? MedicalLicenseNumber { get; set; }
        public decimal Rating { get; set; } = 0;
        public DateTime? ApprovedAt { get; set; }
        public string? ApprovedBy { get; set; }
        public ICollection<DoctorSchedule> DoctorSchedule { get; set; } = new List<DoctorSchedule>();
        public ICollection<DoctorGeneratedSlots> DoctorGeneratedSlots { get; set; } = new List<DoctorGeneratedSlots>();
        public ICollection<Appointment> Appointments { get; set; } = new List<Appointment>();
        public ICollection<Review> Reviews { get; set; } = new List<Review>();
    }
}
