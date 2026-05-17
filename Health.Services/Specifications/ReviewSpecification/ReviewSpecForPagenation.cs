using Health.Domain.Entities.ReviewModule;
using Health.Shared.ReviewSpecParams;
using System;
using System.Collections.Generic;
using System.Text;

namespace Health.Services.Specifications.ReviewSpecification
{
    public class ReviewSpecForPagenation : BaseSpecification<Review , int>
    {
        public ReviewSpecForPagenation(ReviewSpecParam reviewSpec , int patientId) : base(r => r.PatientId == patientId)
        {
            AddInclude(r => r.doctor);
            ApplyPagination(reviewSpec.PageSize, reviewSpec.PageIndex);
        }
    }
}
