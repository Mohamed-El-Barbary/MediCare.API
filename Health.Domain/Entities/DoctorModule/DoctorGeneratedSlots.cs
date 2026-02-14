using System;
using System.Collections.Generic;
using System.Text;

namespace Health.Domain.Entities.DoctorModule
{
    public class DoctorGeneratedSlots : BaseEntity<int>
    {
     
        public int DoctorProfileId { get; set; }
        public DoctorProfile DoctorProfile { get; set; } = default!;

        public int DoctorScheduleId { get; set; }

        public DoctorSchedule DoctorSchedule { get; set; } = default!;

        public DateTime SlotDate { get; set; }

        public TimeSpan StartTime { get; set; }

        public TimeSpan EndTime { get; set; }

        public bool IsBooked { get; set; } = false;

    }
}
