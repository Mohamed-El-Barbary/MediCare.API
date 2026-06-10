using Health.Shared.DTOs.AppointmentDTOs;
using Health.Shared.ParamsForFilterationPatientAppointment;
using System;
using System.Collections.Generic;
using System.Text;

namespace Health.Shared.DTOs.PaymentDTOs
{
    public class PaymentIntentResponseDto
    {
        public string PaymentIntentID { get; set; } = default!;
        public string ClientSecret { get; set; } = default!;
        public decimal Amount { get; set; }
        public int AppointmentId { get; set; }
        public string DoctorName { get; set; } = default!;
        public AppontmentTypeDto AppointmentType { get; set; }
        public AppointmentStatusEnumDTO AppointmentStatus { get; set; }
    }
}
