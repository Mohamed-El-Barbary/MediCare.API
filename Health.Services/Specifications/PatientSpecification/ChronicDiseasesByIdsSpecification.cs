using Health.Domain.Entities.PatientModule;
using System;
using System.Collections.Generic;
using System.Text;

namespace Health.Services.Specifications.PatientSpecification
{
    public class ChronicDiseasesByIdsSpecification : BaseSpecification<ChronicDisease, int>
    {
        public ChronicDiseasesByIdsSpecification(ICollection<int> ids) : base(d => ids.Contains(d.Id))
        {
        }
    }
}
