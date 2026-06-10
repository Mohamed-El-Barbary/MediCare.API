using System;
using System.Collections.Generic;
using System.Text;

namespace Health.Shared.DTOs.ReviewDTOs
{
    public class UpdateReviewDto
    {
        public int Rating { get; set; }
        public string? comment { get; set; }
    }
}
