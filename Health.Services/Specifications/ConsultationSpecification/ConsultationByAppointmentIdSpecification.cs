using Health.Domain.Entities.AppointmentModule;
using Health.Domain.Entities.ConsultationModule;
using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using System.Text;

namespace Health.Services.Specifications.ConsultationSpecification
{
    public class ConsultationByAppointmentIdSpecification : BaseSpecification<Consultation, int>
    {
        public ConsultationByAppointmentIdSpecification(int id) : base(x => x.AppointmentId == id )
        {
        }
    }
}
