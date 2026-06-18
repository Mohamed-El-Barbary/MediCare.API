using Health.Domain.Entities.AppointmentModule;
using System;
using System.Collections.Generic;
using System.Text;

namespace Health.Services.Specifications.AppointmentSpecification
{
    public class TodayAppointmentsCountSpecification : BaseSpecification<Appointment, int>
    {
        public TodayAppointmentsCountSpecification(int doctorId)
            : base(a => a.DoctorProfileId == doctorId && a.DoctorGeneratedSlots.SlotDate == DateTime.Today)
        {
        }
    }
}
