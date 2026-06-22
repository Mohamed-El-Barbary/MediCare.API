using Health.Domain.Entities.AppointmentModule;
using Health.Domain.Entities.ConsultationModule;
using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using System.Text;

namespace Health.Services.Specifications.AppointmentSpecification
{
    public class AppointmentStatisticsSpecification: BaseSpecification<Appointment, int>
    {
        public AppointmentStatisticsSpecification(Expression<Func<Appointment, bool>> criteria): base(criteria)
        {
        }
    }
}
