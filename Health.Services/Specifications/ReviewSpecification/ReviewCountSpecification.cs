using Health.Domain.Entities.ReviewModule;
using System;
using System.Collections.Generic;
using System.Text;

namespace Health.Services.Specifications.ReviewSpecification
{
    public class ReviewCountSpecification : BaseSpecification<Review , int>
    {
        public ReviewCountSpecification(int? DoctorId) : base(r => r.doctorId == DoctorId)
        {}
    }
}
