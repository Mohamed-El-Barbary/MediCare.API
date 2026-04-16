using Health.Domain.Entities.DoctorModule;
using System;
using System.Collections.Generic;
using System.Text;

namespace Health.Domain.Contracts
{
    public interface IDoctorGenerateSlotsRepository 
    {
        public Task<List<DoctorGeneratedSlots>> GetByDoctorAndDateRange(int doctorId, DateTime start, DateTime end);
    }
}
