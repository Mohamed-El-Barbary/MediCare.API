using Health.Domain.Entities.AppointmentModule;
using System;
using System.Collections.Generic;
using System.Text;

namespace Health.Services.Specifications.AppointmentSpecification
{
    public class CompletedAppointmentsSpecification : BaseSpecification<Appointment, int>
    {
        public CompletedAppointmentsSpecification(int doctorId)
         : base(a =>
             a.DoctorProfileId == doctorId &&
             a.Status == AppointmentStatus.AppointmentCompleted)
        {
            AddInclude(a => a.PatientProfile);
            AddInclude(a => a.DoctorGeneratedSlots);

            AddOrderByDesc(a => a.DoctorGeneratedSlots.StartTime);
        }
    }
}
