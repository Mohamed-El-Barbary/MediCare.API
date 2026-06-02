using Health.Domain.Entities.ConsultationModule;
using System;
using System.Collections.Generic;
using System.Text;

namespace Health.Services.Specifications.ConsultationSpecification
{
    public class ConsultationByDoctorIdCountSpecification : BaseSpecification<Consultation, int>
    {
        public ConsultationByDoctorIdCountSpecification(int doctorId) : base(c => c.DoctorId == doctorId)
        {
        }
    }
}
