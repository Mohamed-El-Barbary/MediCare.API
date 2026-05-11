using Health.Domain.Entities.DoctorModule;
using System;
using System.Collections.Generic;
using System.Text;

namespace Health.Services.Specifications.DoctorSceduleSpecification
{
    internal class DoctorByUserIdSpec : BaseSpecification<DoctorProfile, int>
    {
        public DoctorByUserIdSpec(string userId) : base(d => d.UserId == userId)
        {
        }
    }
}
