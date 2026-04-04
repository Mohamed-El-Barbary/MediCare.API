using Health.Domain.Entities.DoctorModule;
using System;
using System.Collections.Generic;
using System.Numerics;
using System.Text;

namespace Health.Services.Specifications.DoctorSpecification
{
    public class DoctorByIdSpecification : BaseSpecification<DoctorProfile, int>
    {
        public DoctorByIdSpecification(string id)
            : base(d => d.UserId == id)
        {
        }
    }
}
