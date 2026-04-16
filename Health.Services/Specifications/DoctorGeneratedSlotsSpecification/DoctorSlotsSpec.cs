using Health.Domain.Entities.DoctorModule;
using Health.Shared.DTOs.Enums;
using System;
using System.Collections.Generic;
using System.Text;

namespace Health.Services.Specifications.DoctorGeneratedSlotsSpecification
{
    public class DoctorSlotsSpec : BaseSpecification<DoctorGeneratedSlots , int> 
    {
        public DoctorSlotsSpec(int doctorId, DateTime? date)
        : base(s =>
            s.DoctorProfileId == doctorId &&
            (!date.HasValue ||
                (s.SlotDate >= date.Value.Date &&
                 s.SlotDate < date.Value.Date.AddDays(1))))
            {
            AddOrderBy(s => s.SlotDate);
        }
    }
}
