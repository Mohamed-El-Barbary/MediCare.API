using Health.Shared.CommonResponses;
using Health.Shared.DTOs.PaymentDTOs;
using System;
using System.Collections.Generic;
using System.Text;

namespace Health.Services.Abstraction.PaymentServiceAbstraction
{
    public interface IPaymentService
    {
        Task<Result<PaymentIntentResponseDto>> CreateOrUpdatePaymentIntentAsync(int AppointmentId);
    }
}
