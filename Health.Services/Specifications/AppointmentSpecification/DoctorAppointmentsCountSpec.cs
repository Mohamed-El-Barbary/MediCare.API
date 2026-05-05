using Health.Domain.Entities.AppointmentModule;
using Health.Shared.ParamsForFilterationPatientAppointment;
using System;
using System.Collections.Generic;
using System.Text;

namespace Health.Services.Specifications.AppointmentSpecification
{
    public class DoctorAppointmentsCountSpec : BaseSpecification<Appointment , int>
    {
        public DoctorAppointmentsCountSpec(int doctorId, AppointmentSpecParams param)
              : base(a =>
                  a.DoctorProfileId == doctorId &&
                  (!param.Status.HasValue || a.Status == (AppointmentStatus)param.Status) &&
                  (!param.Date.HasValue || a.DoctorGeneratedSlots.SlotDate.Date == param.Date.Value.Date)
              )
        {
        }
    }
}
