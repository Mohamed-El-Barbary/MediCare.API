using Health.Domain.Entities.AppointmentModule;
using Health.Domain.Entities.ConsultationModule;
using Health.Shared;
using System;
using System.Collections.Generic;
using System.Text;

namespace Health.Services.Specifications.ConsultationSpecification
{
    public class ConsultationByDoctorIdSpecification : BaseSpecification<Consultation, int>
    {
        public ConsultationByDoctorIdSpecification(int id, ConsultationSpecParams specParams) 
            : base(c => c.DoctorId == id && (string.IsNullOrEmpty(specParams.Status) || c.Status.ToString() == specParams.Status))
        {
            AddInclude(c => c.Doctor);
            AddInclude(c => c.Patient);
            AddOrderByDesc(c => c.ScheduledAt);
            ApplyPagination(specParams.PageSize, specParams.PageIndex);
        }
    }
}
