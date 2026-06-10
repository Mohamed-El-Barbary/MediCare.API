using Health.Domain.Entities.AppointmentModule;
using System;
using System.Collections.Generic;
using System.Text;

namespace Health.Services.Specifications.AppointmentSpecification
{
    public class ExpireAppointmentWithSlots : BaseSpecification<Appointment , int>
    {
        public ExpireAppointmentWithSlots(DateTime expirationData) : base(A => (A.Status == AppointmentStatus.Pending) && (A.PaymentStatus == PaymentStatus.Pending) && (A.CreatedAt <= expirationData))
        {
            AddInclude(a => a.DoctorGeneratedSlots);
        }
    }
}
