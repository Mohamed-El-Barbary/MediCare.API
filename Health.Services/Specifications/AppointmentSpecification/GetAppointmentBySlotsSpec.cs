using Health.Domain.Entities.AppointmentModule;
using System;
using System.Collections.Generic;
using System.Text;

namespace Health.Services.Specifications.AppointmentSpecification
{
    public class GetAppointmentBySlotsSpec : BaseSpecification<Appointment,int>
    {
        public GetAppointmentBySlotsSpec(int appointmentId) : base(a => a.Id == appointmentId)
        {
            AddInclude(a => a.DoctorGeneratedSlots);
        }
    }
}
