using System;
using System.Collections.Generic;
using System.Text;

namespace Health.Domain.Entities.DoctorModule
{
    public class DoctorSchedule : BaseEntity<int>
    {
        public DayOfWeek DayOfWeek { get; set; }

        public TimeSpan StartTime { get; set; }

        public TimeSpan EndTime { get; set; }

        public int SlotDurationMinutes { get; set; }

        public int DoctorProfileId { get; set; }
        public DoctorProfile DoctorProfile { get; set; } = default!;
        public ICollection<DoctorGeneratedSlots> DoctorGeneratedSlots { get; set; } = new List<DoctorGeneratedSlots>();
    }

}
