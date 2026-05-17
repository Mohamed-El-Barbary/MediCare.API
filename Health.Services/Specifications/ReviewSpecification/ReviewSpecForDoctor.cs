using Health.Domain.Entities.ReviewModule;
using System;
using System.Collections.Generic;
using System.Text;

namespace Health.Services.Specifications.ReviewSpecification
{
    public class ReviewSpecForDoctor : BaseSpecification<Review , int>
    {
        public ReviewSpecForDoctor(int DoctorId):base(r => r.doctorId == DoctorId)
        {}
    }
}
