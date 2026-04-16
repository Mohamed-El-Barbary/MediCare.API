using Health.Domain.Contracts;
using Health.Domain.Entities.DoctorModule;
using Health.Persistence.Data.DbContexts;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Text;

namespace Health.Persistence.Repositories.DoctorRepos
{
    public class SlotRepository : IDoctorGenerateSlotsRepository
    {
        private readonly HealthCareDbContext _healthCareDbContext;

        public SlotRepository(HealthCareDbContext healthCareDbContext)
        {
            _healthCareDbContext = healthCareDbContext;
        }
        public async Task<List<DoctorGeneratedSlots>> GetByDoctorAndDateRange(int doctorId, DateTime start, DateTime end)
        {
            return await _healthCareDbContext.DoctorGeneratedSlots
                .Where(
                        s => s.DoctorProfileId == doctorId
                        && s.SlotDate.Date >= start.Date
                        && s.SlotDate.Date <= end.Date
                       ).ToListAsync();
        }
    }
}
