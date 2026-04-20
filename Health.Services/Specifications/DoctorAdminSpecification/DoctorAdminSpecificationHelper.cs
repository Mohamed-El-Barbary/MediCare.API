using Health.Domain.Entities.DoctorModule;
using Health.Services.Specifications.DoctorSpecification;
using Health.Shared;
using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using System.Text;

namespace Health.Services.Specifications.DoctorAdminSpecification
{
    public class DoctorAdminSpecificationHelper
    {
        public static Expression<Func<DoctorProfile, bool>> AdminCriteria(DoctorAdminSpecParams p)
        {
            return d =>
                   (string.IsNullOrEmpty(p.Specialization) || d.Specialization == p.Specialization)
                && (string.IsNullOrEmpty(p.ClinicLocation) || d.ClinicLocation == p.ClinicLocation)
                && (string.IsNullOrEmpty(p.Search) || d.DisplayName.Contains(p.Search))
                && (!(p.MinPrice.HasValue && p.MaxPrice.HasValue) ||
                     (d.PriceConsultation >= p.MinPrice && d.PriceConsultation <= p.MaxPrice))
                && (!p.MinExperience.HasValue || d.YearsOfExperience >= p.MinExperience.Value)
                && (!p.MaxExperience.HasValue || d.YearsOfExperience <= p.MaxExperience.Value)
                && (!p.MinRating.HasValue || d.Rating >= p.MinRating.Value)
                && (!p.VerificationStatus.HasValue || (DoctorVerificationStatus)d.VerificationStatus == p.VerificationStatus.Value);
        }
    }
}
