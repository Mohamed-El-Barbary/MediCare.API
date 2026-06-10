using System;
using System.Collections.Generic;
using System.Text;

namespace Health.Domain.Entities.ConsultationModule
{
    public class PrescriptionItem : BaseEntity<int>
    {
        public int PrescriptionId { get; set; }

        public Prescription Prescription { get; set; } = null!;

        public string? MedicineName { get; set; }

        public string? Dosage { get; set; }

        public string? Frequency { get; set; }

        public int DurationInDays { get; set; }

        public string? Instructions { get; set; }
    }
}
