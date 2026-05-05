using System;
using System.Collections.Generic;
using System.Text;

namespace Health.Shared.DTOs.AppointmentDTOs
{
    public class DoctorAppointmentDTO
    {
        public int Id { get; set; }
        public string PatientName { get; set; } = default!;
        public TimeSpan StartTime { get; set; }
        public TimeSpan EndTime { get; set; }
        public string Status { get; set; } = default!;
        public string AppointmentDate { get; set; } = default!;
        public string Type { get; set; } = default!;
    }
}
