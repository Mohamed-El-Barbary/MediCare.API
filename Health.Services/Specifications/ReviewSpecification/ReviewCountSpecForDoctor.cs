using Health.Domain.Entities.ReviewModule;
using System;
using System.Collections.Generic;
using System.Text;

namespace Health.Services.Specifications.ReviewSpecification
{
    public class ReviewCountSpecForDoctor : BaseSpecification<Review , int>
    {
        public ReviewCountSpecForDoctor(int DoctorId) : base(r => r.doctorId == DoctorId)
        { }
    }
}
