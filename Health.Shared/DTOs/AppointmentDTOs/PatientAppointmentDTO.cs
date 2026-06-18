using System;
using System.Collections.Generic;
using System.Text;

namespace Health.Shared.DTOs.AppointmentDTOs
{
    public class PatientAppointmentDTO
    {
        public int Id { get; set; }
        public string DoctorName { get; set; } = default!;
        public string DoctorSpecialization { get; set; } = default!;
        public string AppointmentDate { get; set; } = default!;
        public TimeSpan StartTime { get; set; }
        public TimeSpan EndTime { get; set; }
        public string Status { get; set; } = default!;
        public string Type { get; set; } = default!;
    }
}
