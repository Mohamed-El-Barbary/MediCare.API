using Health.Domain.Entities.DoctorModule;
using Health.Services.Specifications.DoctorSpecification;
using Health.Shared;
using System;
using System.Collections.Generic;
using System.Text;

namespace Health.Services.Specifications.DoctorAdminSpecification
{
    public class DoctorAdminWithScheduleAndGeneratedSlots : BaseSpecification<DoctorProfile, int>
    {

        public DoctorAdminWithScheduleAndGeneratedSlots(DoctorAdminSpecParams QueryParams)
               : base(DoctorAdminSpecificationHelper.AdminCriteria(QueryParams))
        {
            AddInclude(D => D.DoctorSchedule);
            AddInclude(D => D.DoctorGeneratedSlots);


            switch (QueryParams.Sort)
            {
                case DoctorSortingOptions.Rating:
                    AddOrderByDesc(d => d.Rating);
                    break;
                case DoctorSortingOptions.MaxExperience:
                    AddOrderByDesc(d => d.YearsOfExperience);
                    break;
                case DoctorSortingOptions.MinExperience:
                    AddOrderBy(d => d.YearsOfExperience);
                    break;
                case DoctorSortingOptions.PriceAsc:
                    AddOrderBy(d => d.PriceConsultation);
                    break;
                case DoctorSortingOptions.PriceDesc:
                    AddOrderByDesc(d => d.PriceConsultation);
                    break;
                default:
                    AddOrderBy(d => d.Id);
                    break;
            }

            ApplyPagination(QueryParams.PageSize, QueryParams.PageIndex);
        }

        public DoctorAdminWithScheduleAndGeneratedSlots(int id) : base(X => X.Id == id)
        {
            AddInclude(D => D.DoctorSchedule);
            AddInclude(D => D.DoctorGeneratedSlots);
        }
    }
}
