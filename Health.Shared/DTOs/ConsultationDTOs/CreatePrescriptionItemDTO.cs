using System;
using System.Collections.Generic;
using System.Text;

namespace Health.Shared.DTOs.ConsultationDTOs
{
    public class CreatePrescriptionItemDTO
    {
        public string MedicineName { get; set; } = default!;
        public string Dosage { get; set; } = default!;
        public string Frequency { get; set; } = default!;
        public int DurationInDays { get; set; } = default!;
        public string? Instructions { get; set; }
    }
}
