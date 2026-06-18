using Health.Domain.Entities.AppointmentModule;
using System;
using System.Collections.Generic;
using System.Text;

namespace Health.Services.Specifications.AppointmentSpecification
{
    public class LatestPendingAppointmentSpecification : BaseSpecification<Appointment, int>
    {
        public LatestPendingAppointmentSpecification(int doctorId)
          : base(a =>
              a.DoctorProfileId == doctorId &&
              a.Status == AppointmentStatus.Pending) // ← add this
        {
            AddInclude(a => a.DoctorGeneratedSlots);
            AddInclude(a => a.PatientProfile);

            AddOrderByDesc(a => a.DoctorGeneratedSlots.StartTime);

            ApplyPagination(5, 1);
        }
    }
}
