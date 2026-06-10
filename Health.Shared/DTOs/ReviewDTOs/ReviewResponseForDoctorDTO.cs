using System;
using System.Collections.Generic;
using System.Text;

namespace Health.Shared.DTOs.ReviewDTOs
{
    public class ReviewResponseForDoctorDTO
    {
        public int Rating { get; set; }
        public string? comment { get; set; }
        public string PatientName { get; set; } = default!;
        public int PatientId { get; set; }
    }
}
