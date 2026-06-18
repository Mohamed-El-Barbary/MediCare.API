using Health.Domain.Entities.AppointmentModule;
using System;
using System.Collections.Generic;
using System.Text;

namespace Health.Services.Specifications.AppointmentSpecification
{
    public class DoctorIncomeSpecification : BaseSpecification<Appointment, int>
    {
        public DoctorIncomeSpecification(int doctorId)
            : base(a => a.DoctorProfileId == doctorId && a.PaymentStatus == PaymentStatus.PaymentRecieved)
        {
        }
    }
}
