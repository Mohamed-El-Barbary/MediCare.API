using Health.Domain.Entities.DoctorModule;
using Health.Services.Aggregates;
using Health.Shared;
using System;
using System.Collections.Generic;
using System.Text;

namespace Health.Services.Specifications.DoctorSpecification
{
    public class DoctorWithCountSpecification : BaseSpecification<DoctorProfile, int>
    {
        public DoctorWithCountSpecification(DoctorSpecParams QueryParam)
            : base(
                  DoctorSpecificationHelper.critariaFunc(QueryParam)
            )
        {    
        }
    }
}
