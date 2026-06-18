using Health.Domain.Entities.ConsultationModule;
using System;
using System.Collections.Generic;
using System.Text;

namespace Health.Services.Specifications.ConsultationSpecification
{
    public class PatientRecentConsultationsSpec : BaseSpecification<Consultation , int>
    {
        public PatientRecentConsultationsSpec(int patientId)
        : base(c => c.PatientId == patientId)
        {
            AddInclude(c => c.Prescriptions);
            AddOrderByDesc(c => c.ScheduledAt);
            ApplyPagination(5,1);
        }
    }
}
