using Health.Domain.Entities.AppointmentModule;
using System;
using System.Collections.Generic;
using System.Text;

namespace Health.Services.Specifications.PatientSpecification
{
    public class PatientFilterationById : BaseSpecification<Appointment , int>
    {
        public PatientFilterationById(int patientId):base(p => p.PatientProfileId == patientId)
        {
            AddInclude(p => p.DoctorProfile);
            AddInclude(p => p.DoctorGeneratedSlots);
        }
    }
}
