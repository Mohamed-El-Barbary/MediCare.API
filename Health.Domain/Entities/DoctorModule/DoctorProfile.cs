using System;
using System.Collections.Generic;
using System.Text;

namespace Health.Domain.Entities.DoctorModule
{
    public class DoctorProfile : BaseEntity<int>
    {

        public string UserId { get; set; } = default!;

        public Gender Gender { get; set; }

        public DateTime JoinDate { get; set; } = DateTime.UtcNow;

        public Address Address { get; set; } = default!;

        public string Specialization { get; set; } = default!;

        public string Bio { get; set; } = default!;

        public string ClinicLocation { get; set; } = default!;

        public string SyndicateCardUrl { get; set; } = default!;

        public int YearsOfExperience { get; set; }

        public VerificationStatus VerificationStatus { get; set; } = VerificationStatus.Pending;
        public string DoctorPictureUrl { get; set; } = default!;

        public decimal Rating { get; set; } = 0;

        public DateTime? ApprovedAt { get; set; }

        public string? ApprovedBy { get; set; }

        public ICollection<DoctorSchedule> DoctorSchedule { get; set; } = new List<DoctorSchedule>();

        public ICollection<DoctorGeneratedSlots> DoctorGeneratedSlots { get; set; } = new List<DoctorGeneratedSlots>();
    }
}
