using System;
using System.Collections.Generic;
using System.Text;

namespace Health.Shared.DTOs.AdminDTOs
{
    public class GetDoctorDetailsDTO
    {
        // Identity
        public int Id { get; set; }
        public string? FullName { get; set; }
        public string? Email { get; set; }
        public string? PhoneClinc { get; set; }
        public string? PhoneNumber { get; set; }
        public string?   Gender { get; set; }
        public DateTime JoinDate { get; set; }

        // Doctor Profile
        public string? Specialization { get; set; }
        public int ExperienceYears { get; set; }
        public decimal PriceConsultation { get; set; }
        public double Rating { get; set; }
        public string? ClinicLocation { get; set; }
        public string? Address { get; set; }
        public string? Bio { get; set; }
        public string? SyndicateCardUrl { get; set; }

        // Status
        public string? Status { get; set; } // Approved / Pending / Rejected

        public string? ApprovedBy { get; set; }
        public string? ApprovedByName { get; set; }
        public DateTime? ApprovedAt { get; set; }

        public ICollection<DoctorSlotDTO> Schedule { get; set; } = [];

    }
}
