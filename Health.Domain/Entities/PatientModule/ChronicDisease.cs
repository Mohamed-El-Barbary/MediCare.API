using System;
using System.Collections.Generic;
using System.Text;

namespace Health.Domain.Entities.PatientModule
{
    public class ChronicDisease : BaseEntity<int>
    {
        public string Name { get; set; } = null!;
        public ICollection<PatientChronicDisease> PatientChronicDiseases { get; set; } = new List<PatientChronicDisease>();
    }
}
