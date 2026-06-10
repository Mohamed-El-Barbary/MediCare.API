using Health.Domain.Entities.ConsultationModule;
using System;
using System.Collections.Generic;
using System.Text;

namespace Health.Services.Specifications.ConsultationSpecification
{
    public class ConsultationByDoctorIdSpecification : BaseSpecification<Consultation, int>
    {
        public ConsultationByDoctorIdSpecification(int id, int pageIndex, int pageSize) : base(c => c.DoctorId == id)
        {
            AddInclude(c => c.Doctor);
            AddInclude(c => c.Patient);
            AddOrderByDesc(c => c.ScheduledAt);
            ApplyPagination(pageSize, pageIndex);
        }
    }
}
