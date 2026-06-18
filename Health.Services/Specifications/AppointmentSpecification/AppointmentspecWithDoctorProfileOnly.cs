using Health.Domain.Entities.AppointmentModule;
using System;
using System.Collections.Generic;
using System.Text;

namespace Health.Services.Specifications.AppointmentSpecification
{
    public class AppointmentspecWithDoctorProfileOnly:BaseSpecification<Appointment , int>
    {
        public AppointmentspecWithDoctorProfileOnly(int appointmentId) : base(a => a.Id == appointmentId) 
        {
            AddInclude(a => a.DoctorProfile);
        }
    }
}
