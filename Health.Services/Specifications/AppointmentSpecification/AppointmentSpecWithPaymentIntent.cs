using Health.Domain.Entities.AppointmentModule;
using Stripe.Climate;
using System;
using System.Collections.Generic;
using System.Text;

namespace Health.Services.Specifications.AppointmentSpecification
{
    public class AppointmentSpecWithPaymentIntent : BaseSpecification<Appointment, int>
    {
        public AppointmentSpecWithPaymentIntent(string paymentIntentId) : base(O => O.PaymentIntentID == paymentIntentId)
        {

        }
    }
}
