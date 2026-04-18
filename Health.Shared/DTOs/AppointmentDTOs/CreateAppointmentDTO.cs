using System;
using System.Collections.Generic;
using System.Text;

namespace Health.Shared.DTOs.AppointmentDTOs
{
    public class CreateAppointmentDTO
    {
        public int DoctorProfileId { get; set; }
        public int DoctorGeneratedSlotsId { get; set; }
        public AppontmentTypeDto AppointmentType{ get; set; }
    }
}
