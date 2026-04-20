using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.Text;

namespace Health.Shared.DTOs.AdminDTOs
{
    public class GetAllDoctorsDTO
    {
        public int Id { get; set; }
        public string DoctorPictureUrl { get; set; } = default!;
        public string DisplayName { get; set; } = default!;
        public string Specialization { get; set; } = default!;
        public int YearsOfExperience { get; set; }
        public decimal Rating { get; set; } = 0;
        public decimal PriceConsultation { get; set; }
        public string VerificationStatus { get; set; } = default!;
        public DateTime JoinDate { get; set; }
    }
}
