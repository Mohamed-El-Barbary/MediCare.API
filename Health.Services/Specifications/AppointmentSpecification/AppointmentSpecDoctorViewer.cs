using Health.Domain.Entities.AppointmentModule;
using System;
using System.Collections.Generic;
using System.Text;

namespace Health.Services.Specifications.AppointmentSpecification
{
    public class AppointmentSpecDoctorViewer : BaseSpecification<Appointment , int>
    {
        public AppointmentSpecDoctorViewer(int AppointmentId) : base(a => a.Id == AppointmentId)
        {
            AddInclude(a => a.PatientProfile);
            AddInclude(a => a.DoctorGeneratedSlots);
        }
    }
}
