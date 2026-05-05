using Health.Domain.Entities.PatientModule;
using System;
using System.Collections.Generic;
using System.Text;

namespace Health.Services.Specifications.PatientSpecification
{
    public class PatientByIdWithoutIncludes : BaseSpecification<PatientProfile, int>
    {
        public PatientByIdWithoutIncludes(string userId) : base(p => p.UserId == userId) 
        {
            
        }

    }
}
