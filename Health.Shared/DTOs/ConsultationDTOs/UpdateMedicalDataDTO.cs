using System;
using System.Collections.Generic;
using System.Text;

namespace Health.Shared.DTOs.ConsultationDTOs
{
    public class UpdateMedicalDataDTO
    {
        public string? Symptoms { get; set; }

        public string? Notes { get; set; }

        public string? Diagnosis { get; set; }

    }
}
