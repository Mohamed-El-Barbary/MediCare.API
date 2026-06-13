using System;
using System.Collections.Generic;
using System.Text;

namespace Health.Domain.Entities.ConsultationModule
{
    public class Prescription : BaseEntity<int>
    {
        public int ConsultationId { get; set; }
        public Consultation Consultation { get; set; } = default!;
        public string? AdditionalNotes { get; set; }
        public ICollection<PrescriptionItem> Items { get; set; } = new List<PrescriptionItem>();
    }
}
