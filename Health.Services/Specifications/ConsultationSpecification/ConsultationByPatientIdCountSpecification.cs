using Health.Domain.Entities.ConsultationModule;
using System;
using System.Collections.Generic;
using System.Text;

namespace Health.Services.Specifications.ConsultationSpecification
{
    public class ConsultationByPatientIdCountSpecification : BaseSpecification<Consultation,int>
    {
        public ConsultationByPatientIdCountSpecification(int id) : base(c => c.PatientId == id)
        {
        }
    }
}
