using Health.Domain.Entities.DoctorModule;
using System;
using System.Collections.Generic;
using System.Text;

namespace Health.Services.Specifications.DoctorSpecification
{
    public class DoctorWithScheduleAndGeneratedSlots : BaseSpecification<DoctorProfile , int>
    {
        public DoctorWithScheduleAndGeneratedSlots() : base(null)
        {
            AddInclude(D => D.DoctorSchedule);
            AddInclude(D => D.DoctorGeneratedSlots);
        }

        public DoctorWithScheduleAndGeneratedSlots(int id) : base(X => X.Id == id)
        {
            AddInclude(D => D.DoctorSchedule);
            AddInclude(D => D.DoctorGeneratedSlots);
        }
    }
}
