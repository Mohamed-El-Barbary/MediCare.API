using Health.Domain.Entities.AppointmentModule;
using Health.Shared.ParamsForFilterationPatientAppointment;
using System;
using System.Collections.Generic;
using System.Text;

namespace Health.Services.Specifications.AppointmentSpecification
{
    public class PatientAppointmentsCountSpec : BaseSpecification<Appointment , int>
    {
        public PatientAppointmentsCountSpec(int patientId, AppointmentSpecParams param)
            : base(a =>
                a.PatientProfileId == patientId &&
                (!param.Status.HasValue || a.Status == (AppointmentStatus)param.Status) &&
                (!param.Date.HasValue || a.DoctorGeneratedSlots.SlotDate.Date == param.Date.Value.Date)
            )
        {
        }
    }
}
