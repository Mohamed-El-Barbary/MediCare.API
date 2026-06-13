using Health.Domain.Entities.AppointmentModule;
using System;
using System.Collections.Generic;
using System.Text;

namespace Health.Services.Specifications.AppointmentSpecification
{
    public class TodayAppointmentsSpecification : BaseSpecification<Appointment, int>
    {
        private static readonly DateTime Today = DateTime.Today;
        private static readonly DateTime Tomorrow = DateTime.Today.AddDays(1);

        public TodayAppointmentsSpecification(int doctorId)
            : base(a =>
                a.DoctorProfileId == doctorId &&
                a.DoctorGeneratedSlots.SlotDate >= Today &&
                a.DoctorGeneratedSlots.SlotDate < Tomorrow)
        {
            AddInclude(a => a.PatientProfile);
            AddInclude(a => a.DoctorGeneratedSlots);
            AddOrderBy(a => a.DoctorGeneratedSlots.StartTime);
            ApplyPagination(5, 1);
        }
    }
}
