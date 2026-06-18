using Health.Domain.Entities.ConsultationModule;
using Health.Domain.Entities.DoctorModule;
using Health.Shared;
using System;
using System.Collections.Generic;
using System.Text;

namespace Health.Services.Specifications.ConsultationSpecification
{
    internal class ConsultationByPatientIdSpecification : BaseSpecification<Consultation, int>
    {
        public ConsultationByPatientIdSpecification(int id, ConsultationSpecParams specParams)
            : base(c => c.PatientId == id && (string.IsNullOrEmpty(specParams.Status) || c.Status.ToString() == specParams.Status))
        {
            AddInclude(c => c.Doctor);
            AddInclude(c => c.Patient);
            AddOrderByDesc(c => c.ScheduledAt);

            ApplyPagination(specParams.PageSize, specParams.PageIndex);
        }
    }
}
