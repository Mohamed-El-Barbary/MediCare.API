using Health.Domain.Entities.ConsultationModule;
using Health.Domain.Entities.DoctorModule;
using System;
using System.Collections.Generic;
using System.Text;

namespace Health.Services.Specifications.ConsultationSpecification
{
    internal class ConsultationByPatientIdSpecification : BaseSpecification<Consultation, int>
    {
        public ConsultationByPatientIdSpecification(int id, int pageIndex, int pageSize) : base(c => c.PatientId == id)
        {
            AddInclude(c => c.Doctor);
            AddInclude(c => c.Patient);
            AddOrderByDesc(c => c.ScheduledAt);

            ApplyPagination(pageSize, pageIndex);
        }
    }
}
