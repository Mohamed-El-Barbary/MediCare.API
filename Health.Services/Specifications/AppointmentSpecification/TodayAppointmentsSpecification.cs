using Health.Domain.Entities.AppointmentModule;
using System;
using System.Collections.Generic;
using System.Text;

namespace Health.Services.Specifications.AppointmentSpecification
{
    public class TodayAppointmentsSpecification : BaseSpecification<Appointment, int>
    {
        public TodayAppointmentsSpecification(int doctorId)
            : base(a => a.DoctorProfileId == doctorId 
            &&a.DoctorGeneratedSlots.StartTime >= DateTime.Now.TimeOfDay 
            && a.DoctorGeneratedSlots.StartTime < DateTime.Now.AddDays(1).TimeOfDay)
        {
            AddOrderBy(a => a.DoctorGeneratedSlots.StartTime);
            ApplyPagination(0, 5);
        }
    }
}
