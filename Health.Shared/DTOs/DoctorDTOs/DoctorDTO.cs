using System;
using System.Collections.Generic;
using System.Text;

namespace Health.Shared.DTOs.DoctorDTOs
{
    public record DoctorDTO
    {
        public int DoctorId { get; set; }
        public string FisrtName { get; set; } = default!;
        public string LastName { get; set; } = default!;
        public string Specialization { get; init; } = default!;
        public int YearsOfExperience { get; init; }
        public string Bio { get; init; } = default!;
        public string DoctorPictureUrl { get; init; } = default!;

        public List<DoctorScheduleDTO>? DoctorScheduleDTO { get; set; }

        public List<GeneratedSlotsDTO>? GeneratedSlotsDTO { get; set; }
    }
}
