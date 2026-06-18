using Health.Domain.Entities.AppointmentModule;
using System;
using System.Collections.Generic;
using System.Text;

namespace Health.Services.Specifications.AppointmentSpecification
{
    public class AppointmentSpecPatientViewer: BaseSpecification<Appointment , int>
    {
        public AppointmentSpecPatientViewer(int AppointmentId) : base(a => a.Id == AppointmentId)
        {
            AddInclude(a => a.DoctorProfile);
            AddInclude(a => a.DoctorGeneratedSlots);
        }
    }
}
