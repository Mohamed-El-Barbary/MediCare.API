using Health.Domain.Entities.DoctorModule;
using System;
using System.Collections.Generic;
using System.Text;

namespace Health.Services.Specifications.DoctorSceduleSpecification
{
    public class DoctorSceduleByDoctorProfileIdSpec : BaseSpecification<DoctorSchedule , int> 
    {
        public DoctorSceduleByDoctorProfileIdSpec(int id, int? excludeScheduleId = null)
       : base(s => s.DoctorProfileId == id
                   && (!excludeScheduleId.HasValue || s.Id != excludeScheduleId.Value))
        {
        }
    }
}
