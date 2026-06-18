using System;
using System.Collections.Generic;
using System.Text;

namespace Health.Shared.DTOs.ReviewDTOs
{
    public class CreateReviewDto
    {
        public int AppointmentId { get; set; }
        public int Rating { get; set; }
        public string? Comment { get; set; } = default!;
    }
}
