using Health.Domain.Entities.ConsultationModule;
using System;
using System.Collections.Generic;
using System.Text;

namespace Health.Services.Specifications.ConsultationSpecification
{
    public class ConsultationByIdSpecification : BaseSpecification<Consultation,int>
    {
        public ConsultationByIdSpecification(int id) :base(c => c.Id == id)
        {
            AddInclude(c => c.Prescriptions);
            AddInclude(c => c.Connections);
            AddInclude("Prescriptions.Items");
            AddInclude(c => c.Doctor);
            AddInclude(c => c.Patient);
        }
    }
}
