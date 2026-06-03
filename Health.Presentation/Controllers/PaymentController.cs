using Health.Services.Abstraction.PaymentServiceAbstraction;
using Health.Shared.DTOs.PaymentDTOs;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Text;

namespace Health.Presentation.Controllers
{
    public class PaymentController : ApiBaseController
    {
        private readonly IPaymentService _paymentService;

        public PaymentController(IPaymentService paymentService)
        {
            _paymentService = paymentService;
        }

        [Authorize(Roles = "Patient")]
        [HttpPost("{appointmentId}")]
        public async Task<ActionResult<PaymentIntentResponseDto>> CreateOrUpdatePaymentIntent(int appointmentId)
        {
            var result = await _paymentService.CreateOrUpdatePaymentIntentAsync(appointmentId);
            return HandleResult(result);
        }

        [HttpPost("webhock")]
        public async Task<IActionResult> WebHock()
        {
            var json = await new StreamReader(HttpContext.Request.Body).ReadToEndAsync();
            var stripeSignature = Request.Headers["Stripe-Signature"];
            await _paymentService.UpdateAppointmentPaymentStatus(json, stripeSignature!);
            return new EmptyResult();
        }

    }
}
