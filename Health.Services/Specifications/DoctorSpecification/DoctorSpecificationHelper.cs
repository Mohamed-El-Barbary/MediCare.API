using Health.Domain.Entities.DoctorModule;
using Health.Services.Aggregates;
using Health.Shared;
using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using System.Text;

namespace Health.Services.Specifications.DoctorSpecification
{
    public static class DoctorSpecificationHelper
    {
        public static Expression<Func<DoctorProfile, bool>> critariaFunc(DoctorSpecParams queryParams)
        {
            return d =>
                          (string.IsNullOrEmpty(queryParams.Specialization) || d.Specialization == queryParams.Specialization)
                       && (string.IsNullOrEmpty(queryParams.ClinicLocation) || d.ClinicLocation == queryParams.ClinicLocation)
                       && (string.IsNullOrEmpty(queryParams.Search) || d.DisplayName.ToLower().Contains(queryParams.Search.ToLower()))
                       &&  (!(queryParams.MinPrice.HasValue && queryParams.MaxPrice.HasValue) 
                            ||
                           (
                             d.PriceConsultation >= queryParams.MinPrice.Value 
                              &&  
                             d.PriceConsultation <= queryParams.MaxPrice.Value)
                           );

        }
    }
}
