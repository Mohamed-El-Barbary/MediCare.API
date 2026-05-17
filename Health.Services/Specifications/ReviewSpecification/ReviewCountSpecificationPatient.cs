using Health.Domain.Entities.ReviewModule;
using System;
using System.Collections.Generic;
using System.Text;

namespace Health.Services.Specifications.ReviewSpecification
{
    public class ReviewCountSpecificationPatient : BaseSpecification<Review , int>
    {
        public ReviewCountSpecificationPatient(int PatientId) : base(r => r.PatientId == PatientId)
        {}
    }
}
