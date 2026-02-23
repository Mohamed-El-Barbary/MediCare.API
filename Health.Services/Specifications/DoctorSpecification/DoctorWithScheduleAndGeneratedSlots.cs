using Health.Domain.Entities.DoctorModule;
using Health.Shared;
using System;
using System.Collections.Generic;
using System.Text;

namespace Health.Services.Specifications.DoctorSpecification
{
    public class DoctorWithScheduleAndGeneratedSlots : BaseSpecification<DoctorProfile , int>
    {
        public DoctorWithScheduleAndGeneratedSlots(DoctorSpecParams QueryParams) 
               : base(DoctorSpecificationHelper.critariaFunc(QueryParams))
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
                default:
                    AddOrderBy(d => d.Id);
                    break;
            }

            ApplyPagination(QueryParams.PageSize, QueryParams.PageIndex);
        }

        public DoctorWithScheduleAndGeneratedSlots(int id) : base(X => X.Id == id)
        {
            AddInclude(D => D.DoctorSchedule);
            AddInclude(D => D.DoctorGeneratedSlots);
        }
    }
}
