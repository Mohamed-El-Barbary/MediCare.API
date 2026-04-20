using Health.Domain.Entities.DoctorModule;
using Health.Services.Specifications.DoctorSpecification;
using Health.Shared;
using System;
using System.Collections.Generic;
using System.Text;

namespace Health.Services.Specifications.DoctorAdminSpecification
{
    internal class DoctorAdminWithCountSpecification : BaseSpecification<DoctorProfile, int>
    {
        public DoctorAdminWithCountSpecification(DoctorAdminSpecParams QueryParam)
            : base(
                  DoctorAdminSpecificationHelper.AdminCriteria(QueryParam)
            )
        {
        }
    }
}
