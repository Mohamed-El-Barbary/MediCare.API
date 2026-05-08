using Health.Domain.Entities.PatientModule;
using System;
using System.Collections.Generic;
using System.Text;

namespace Health.Services.Specifications.PatientSpecification
{
    public class PatientByIdSpecification : BaseSpecification<PatientProfile, int>
    {
        public PatientByIdSpecification(string id) : base(p => p.UserId == id)
        {
            AddInclude(p => p.PatientChronicDiseases);
        }
    }
}
