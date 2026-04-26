using Health.Domain.Entities.AppointmentModule;
using Health.Shared.ParamsForFilterationPatientAppointment;
using System;
using System.Collections.Generic;
using System.Text;

namespace Health.Services.Specifications.PatientSpecification
{
    public class PatientFilteration : BaseSpecification<Appointment , int>
    {
        public PatientFilteration(int patientId, AppointmentSpecParams param)
            :base(
                  p => p.PatientProfileId == patientId &&
                  (!param.Status.HasValue || p.Status == (AppointmentStatus)param.Status) &&
                  (!param.Date.HasValue || p.DoctorGeneratedSlots.SlotDate.Date == param.Date.Value.Date)
                 )
        {
            AddInclude(p => p.DoctorProfile);
            AddInclude(p => p.DoctorGeneratedSlots);


            ApplyPagination(param.PageSize , param.PageIndex);
        }
    }
}
