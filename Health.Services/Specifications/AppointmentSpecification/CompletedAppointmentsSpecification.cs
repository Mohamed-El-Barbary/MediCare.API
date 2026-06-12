using Health.Domain.Entities.AppointmentModule;
using System;
using System.Collections.Generic;
using System.Text;

namespace Health.Services.Specifications.AppointmentSpecification
{
    public class CompletedAppointmentsSpecification : BaseSpecification<Appointment, int>
    {
        public CompletedAppointmentsSpecification(int doctorId)
            : base(a => a.DoctorProfileId == doctorId && a.Status == AppointmentStatus.AppointmentCompleted)
        {
            AddOrderByDesc(a => a.DoctorGeneratedSlots.StartTime);
            AddInclude(a => a.PatientProfile);
        }
    }
}
