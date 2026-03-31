using Health.Domain.Entities.DoctorModule;
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

        public ICollection<PatientChronicDisease> PatientChronicDiseases = [];
    }
}
