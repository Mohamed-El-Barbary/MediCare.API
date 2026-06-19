using Health.Domain.Entities.ConsultationModule;
using Health.Domain.Entities.ConsultationModule.Enums;
using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using System.Text;

namespace Health.Services.Specifications.ConsultationSpecification
{
    public class ConsultationStatisticsSpecification
        : BaseSpecification<Consultation, int>
    {
        public ConsultationStatisticsSpecification(
            Expression<Func<Consultation, bool>> criteria)
            : base(criteria)
        {
        }
    }
}
