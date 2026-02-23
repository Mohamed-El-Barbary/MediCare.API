using System;
using System.Collections.Generic;
using System.Text;

namespace Health.Shared.DTOs.DoctorDTOs
{
    public record DoctorDTO
    {
        public int DoctorId { get; set; }
        public string DisplayName { get; set; } = default!;
        public string Specialization { get; init; } = default!;
        public int YearsOfExperience { get; init; }
        public string Bio { get; init; } = default!;
        public string DoctorPictureUrl { get; init; } = default!;
        public decimal Rating { get; set; }
        public string PhoneNumber { get; set; } = default!;
        public DoctorScheduleDTO? DoctorScheduleDTO { get; set; }
        public GeneratedSlotsDTO? GeneratedSlotsDTO { get; set; }
    }
}
