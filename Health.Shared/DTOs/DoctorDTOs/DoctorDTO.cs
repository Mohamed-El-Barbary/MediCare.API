using System;
using System.Collections.Generic;
using System.Text;

namespace Health.Shared.DTOs.DoctorDTOs
{
    public record DoctorDTO
    {
        public int Id { get; set; }
        public string DisplayName { get; set; } = default!;
        public string Specialization { get; init; } = default!;
        public string YearsOfExperience { get; init; } = default!;
        public string? Bio { get; init; }
        public string? DoctorPictureUrl { get; init; }
        public decimal PriceConsultation { get; set; }
        public decimal Rating { get; set; }
        public string? PhoneClinc { get; set; }
        public string ClinicLocation { get; set; } = default!;
        public List<DoctorScheduleDTO>? DoctorScheduleDTO { get; set; }
        public List<GeneratedSlotsDTO>? GeneratedSlotsDTO { get; set; }
    }
}
