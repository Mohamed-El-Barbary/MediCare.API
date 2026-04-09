using Health.Domain.Entities.DoctorModule;
using System;
using System.Collections.Generic;
using System.Text;

namespace Health.Services.Specifications.DoctorSceduleSpecification
{
    public class DoctorScheduleByDoctorProfileExcludingScheduleSpec : BaseSpecification<DoctorSchedule, int>
    {
        public DoctorScheduleByDoctorProfileExcludingScheduleSpec(int doctorProfileId, int excludeScheduleId)
        : base(s => s.DoctorProfileId == doctorProfileId && s.Id != excludeScheduleId)
        {
        }
    }
}
