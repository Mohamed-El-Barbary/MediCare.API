using System;
using System.Collections.Generic;
using System.Text;

namespace Health.Shared.DTOs.ConsultationDTOs
{
    public class PrescriptionDTO
    {
        public int Id { get; set; }
        public int ConsultationId { get; set; }
        public string? AdditionalNotes { get; set; }
        public DateTime IssuedAt { get; set; }
        public ICollection<PrescriptionItemDTO> Items { get; set; }
            = new List<PrescriptionItemDTO>();
    }
}
