using Health.Domain.Entities.AppointmentModule;
using System;
using System.Collections.Generic;
using System.Text;

namespace Health.Services.Specifications.AppointmentSpecification
{
    public class ConfirmedAndPendingAppointmentSpec : BaseSpecification<Appointment , int>
    {
            public ConfirmedAndPendingAppointmentSpec(int PatientId)
             : base(
                 a =>
                 a.PatientProfileId == PatientId && (
                 a.Status == AppointmentStatus.AppointmentConfirmed ||
                 a.Status == AppointmentStatus.Pending)
             )
            {

                AddInclude(a => a.PatientProfile);
                AddInclude(a => a.DoctorProfile);
                AddInclude(a => a.DoctorGeneratedSlots);
                AddOrderByDesc(a => a.DoctorGeneratedSlots.StartTime);
            }
        
    }
}
