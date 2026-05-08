using Health.Shared.DTOs.Enums;
using System;
using System.Collections.Generic;
using System.Text;

namespace Health.Shared.DTOs.DoctorDTOs
{
    public class GeneratedSlotsDTO
    {
        public int Id { get; set; }
        public DateTime SlotDate { get; set; }
        public TimeSpan StartTime { get; set; }
        public TimeSpan EndTime { get; set; }
        public EnumSlotStatusDTO Status { get; set; }
    }
}
