using Health.Domain.Entities.DoctorModule;
using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using System.Text;

namespace Health.Domain.Contracts
{
    public interface IDoctorScheduleRepository
    {
        // Check Dublicate Day Of DoctorSchedule
        Task<bool> ExistsAsync(Expression<Func<DoctorSchedule, bool>> predicate);
    }
}
