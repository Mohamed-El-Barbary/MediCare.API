using System;
using System.Collections.Generic;
using System.Text;

namespace Health.Shared.DTOs.ReviewDTOs
{
    public class ReviewResposeDTO
    {
        public int Id { get; set; }
        public int Rating { get; set; }
        public string? Comment { get; set; }
        public int DoctorId { get; set; }
        public DateTime CreatedAt { get; set; }
    }
}
