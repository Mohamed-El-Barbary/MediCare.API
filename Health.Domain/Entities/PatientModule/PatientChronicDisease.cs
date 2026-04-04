using System;
using System.Collections.Generic;
using System.Text;

namespace Health.Domain.Entities.PatientModule
{
    public class PatientChronicDisease
    {
        public int PatientId { get; set; }
        public PatientProfile Patient { get; set; } = null!;

        public int ChronicDiseaseId { get; set; }
        public ChronicDisease ChronicDisease { get; set; } = null!;

        public DateTime DiagnosedAt { get; set; } = DateTime.UtcNow;
        public bool IsActive { get; set; } = true;
    }
}
