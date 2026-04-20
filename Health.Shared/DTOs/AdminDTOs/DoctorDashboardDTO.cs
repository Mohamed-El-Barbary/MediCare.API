using System;
using System.Collections.Generic;
using System.Text;

namespace Health.Shared.DTOs.AdminDTOs
{
    public class DoctorDashboardDTO
    {
        public PaginatedResult<GetAllDoctorsDTO>? DoctorsData { get; set; }
        public DoctorAnalyticsDataDTO? Analytics { get; set; }
        public DoctorAdminSpecParams QueryParams { get; set; } = new(); // <= هنا

    }
}
