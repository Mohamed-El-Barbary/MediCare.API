using Health.Domain.Entities.AppointmentModule;
using Health.Shared.ParamsForFilterationPatientAppointment;
using System;
using System.Collections.Generic;
using System.Text;

namespace Health.Services.Specifications.AppointmentSpecification
{
    public class DoctorAppointmentFilteration : BaseSpecification<Appointment , int>
    {
        public DoctorAppointmentFilteration(int doctorId, AppointmentSpecParams param)
            : base(
                  a => a.DoctorProfileId == doctorId &&
                  (!param.Status.HasValue || a.Status == (AppointmentStatus)param.Status) &&
                  (!param.Date.HasValue || a.DoctorGeneratedSlots.SlotDate.Date == param.Date.Value.Date)
                 )
        {
            AddInclude(p => p.PatientProfile);
            AddInclude(p => p.DoctorGeneratedSlots);


            ApplyPagination(param.PageSize, param.PageIndex);
        }
    }
}
