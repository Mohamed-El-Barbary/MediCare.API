using Health.Domain.Entities.ConsultationModule;
using System;
using System.Collections.Generic;
using System.Text;

namespace Health.Services.Specifications.ConsultationSpecification
{
    public class PrescriptionByIdSpecification : BaseSpecification<Prescription, int>
    {
        public PrescriptionByIdSpecification(int id) : base(p => p.Id == id)
        {
            AddInclude(p => p.Items);
        }
    }
}
