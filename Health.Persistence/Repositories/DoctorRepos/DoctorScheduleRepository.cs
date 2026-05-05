using Health.Domain.Contracts;
using Health.Domain.Entities.DoctorModule;
using Health.Persistence.Data.DbContexts;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using System.Text;

namespace Health.Persistence.Repositories.DoctorRepos
{
    public class DoctorScheduleRepository : IDoctorScheduleRepository
    {
        private readonly HealthCareDbContext _healthCareDbContext;

        public DoctorScheduleRepository(HealthCareDbContext healthCareDbContext)
        {
            _healthCareDbContext = healthCareDbContext;
        }

        public async Task<bool> ExistsAsync(Expression<Func<DoctorSchedule, bool>> predicate)
        {
            return await _healthCareDbContext.DoctorSchedules.AnyAsync(predicate);
        }

    }
}
