using System;
using System.Collections.Generic;
using System.Text;

namespace Health.Shared.DTOs.AdminDTOs
{
    public class DoctorAnalyticsDataDTO
    {
        public int TotalDoctors { get; set; }
        public int ActiveDoctors { get; set; }
        public decimal DoctorsAvgRating { get; set; }
        public int PendingDoctors { get; set; }
    }
}
