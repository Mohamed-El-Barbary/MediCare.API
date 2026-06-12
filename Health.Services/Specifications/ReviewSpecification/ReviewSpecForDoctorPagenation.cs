using Health.Domain.Entities.ReviewModule;
using Health.Shared.ReviewSpecParams;
using System;
using System.Collections.Generic;
using System.Text;

namespace Health.Services.Specifications.ReviewSpecification
{
    public class ReviewSpecForDoctorPagenation : BaseSpecification<Review , int>
    {
        public ReviewSpecForDoctorPagenation(ReviewSpecParam reviewSpec, int doctorId) : base(r => r.doctorId == doctorId)
        {
            AddInclude(r => r.Patient);
            ApplyPagination(reviewSpec.PageSize, reviewSpec.PageIndex);
        }
    }
}
