using Health.Domain.Entities.DoctorModule;
using System;
using System.Collections.Generic;
using System.Text;

namespace Health.Services.Specifications.DoctorSpecification
{
    public class ExpiredSlotsSpec : BaseSpecification<DoctorGeneratedSlots , int>
    {
        public ExpiredSlotsSpec() : base(s => s.Status == SlotStatus.Available)
        {}
    }
}
