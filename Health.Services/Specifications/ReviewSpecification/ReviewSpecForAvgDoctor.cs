using Health.Domain.Entities.ReviewModule;
using System;
using System.Collections.Generic;
using System.Text;

namespace Health.Services.Specifications.ReviewSpecification
{
    public class ReviewSpecForAvgDoctor : BaseSpecification<Review , int>
    {
        public ReviewSpecForAvgDoctor(int DoctorId):base(r => r.doctorId == DoctorId)
        {}
    }
}
