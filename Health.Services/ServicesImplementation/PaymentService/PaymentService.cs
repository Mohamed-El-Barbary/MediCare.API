using CloudinaryDotNet.Actions;
using Health.Domain.Contracts;
using Health.Domain.Entities.AppointmentModule;
using Health.Services.Abstraction.ConsultationModule;
using Health.Services.Abstraction.PaymentServiceAbstraction;
using Health.Services.Specifications.AppointmentSpecification;
using Health.Shared.CommonResponses;
using Health.Shared.DTOs.AppointmentDTOs;
using Health.Shared.DTOs.ConsultationDTOs;
using Health.Shared.DTOs.PaymentDTOs;
using Health.Shared.ParamsForFilterationPatientAppointment;
using Microsoft.Extensions.Configuration;
using SendGrid.Helpers.Mail;
using Stripe;
using Stripe.Climate;
using System;
using System.Collections.Generic;
using System.Text;
using Error = Health.Shared.CommonResponses.Error;

namespace Health.Services.ServicesImplementation.PaymentService
{
    public class PaymentService : IPaymentService
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IConsultationService _consultationService;
        private readonly IConfiguration _configuration;

        public PaymentService(IUnitOfWork unitOfWork ,IConsultationService consultationService ,IConfiguration configuration)
        {
            _unitOfWork = unitOfWork;
            _consultationService = consultationService;
            _configuration = configuration;
        }
        public async Task<Result<PaymentIntentResponseDto>> CreateOrUpdatePaymentIntentAsync(int AppointmentId)
        {
            var skey = _configuration["Stripe:SKey"];
            if (skey is null)
                return Error.Failure("Failed To Obtain Secret Key Value");
            StripeConfiguration.ApiKey = skey;

            var AppointmentWithDoctorProfileSpec = new AppointmentspecWithDoctorProfileOnly(AppointmentId);
            var appointment = await _unitOfWork.GetRepository<Appointment, int>().GetByIdAsync(AppointmentWithDoctorProfileSpec);
            if (appointment is null)
                return Error.NotFound("Appointment With This Id Is Not Found");

            if (appointment.DoctorProfile is null)
                return Error.NotFound("Doctor profile not found for this appointment.");
            // Validations
            if (appointment.PaymentStatus == PaymentStatus.PaymentRecieved)
                return Error.Validation("Payment has already been received for this appointment.");

            if (appointment.Status == AppointmentStatus.AppointmentCancelled)
                return Error.Validation("Cannot pay for cancelled  appointment");

            if (appointment.Status == AppointmentStatus.AppointmentConfirmed)
                return Error.Validation("Cannot pay for confirmed  appointment");

            if (appointment.Status == AppointmentStatus.AppointmentCompleted)
                return Error.Validation("Cannot pay for Completed  appointment");

            // Get Price Consultation From DoctorProfile
            long amount = (long) appointment.DoctorProfile.PriceConsultation * 100;

            //  Create Or Update Payment Intent With Stripe API
            PaymentIntent paymentIntent;
            var stripeService = new PaymentIntentService();
            if(appointment.PaymentIntentID is null)
            {
                var options = new PaymentIntentCreateOptions
                {
                    Amount = amount,
                    Currency = "USD",
                    PaymentMethodTypes = ["card"]
                };

                paymentIntent = await stripeService.CreateAsync(options);
                appointment.PaymentIntentID = paymentIntent.Id;
                appointment.Amount = amount;
            }
            else
            {
                var options = new PaymentIntentUpdateOptions
                {
                    Amount = amount,
                };
                paymentIntent = await stripeService.UpdateAsync(appointment.PaymentIntentID, options);

            }

             _unitOfWork.GetRepository<Appointment, int>().Update(appointment);

             await _unitOfWork.SaveChanges();

            var dto = new PaymentIntentResponseDto
            {
                PaymentIntentID = paymentIntent.Id,
                ClientSecret = paymentIntent.ClientSecret,
                Amount = amount,

                AppointmentId = appointment.Id,
                DoctorName = appointment.DoctorProfile.DisplayName,

                AppointmentType = (AppontmentTypeDto) appointment.AppointmentType,
                AppointmentStatus = (AppointmentStatusEnumDTO) appointment.Status
            };

            return Result<PaymentIntentResponseDto>.Ok(dto);
        }

        public async Task UpdateAppointmentPaymentStatus(string request, string stripeSignature)
        {
            var endpointSecret = _configuration["Stripe:EndpointSecret"];
            var stripeEvent = EventUtility.ParseEvent(request, throwOnApiVersionMismatch: true);
            stripeEvent = EventUtility.ConstructEvent(request, stripeSignature, endpointSecret, throwOnApiVersionMismatch: true);

            var paymentIntent = stripeEvent.Data.Object as PaymentIntent;
            Console.WriteLine(paymentIntent!.Id);
            var Appointment = await _unitOfWork.GetRepository<Appointment, int>().GetByIdAsync(new AppointmentSpecWithPaymentIntent(paymentIntent!.Id));

            var createConsultationDto = new CreateConsultationDTO()
            {
                AppointmentId = Appointment!.Id,
                DoctorId = Appointment.DoctorProfileId,
                PatientId = Appointment.PatientProfileId,
                Type = 0,
            };

            // Handle the event
            if (stripeEvent.Type == EventTypes.PaymentIntentSucceeded)
            {
                Appointment!.PaymentStatus = PaymentStatus.PaymentRecieved;
                Appointment.Status = AppointmentStatus.AppointmentConfirmed;
                Appointment.PaidAt = DateTime.Now;
                _unitOfWork.GetRepository<Appointment, int>().Update(Appointment);
                await _consultationService.CreateAsync(createConsultationDto);
                await _unitOfWork.SaveChanges();
            }
            else if (stripeEvent.Type == EventTypes.PaymentIntentPaymentFailed)
            {
                Appointment!.PaymentStatus = PaymentStatus.PaymentFaild;
                _unitOfWork.GetRepository<Appointment, int>().Update(Appointment);
                await _unitOfWork.SaveChanges();
            }
            // ... handle other event types
            else
            {
                Console.WriteLine("Unhandled event type: {0}", stripeEvent.Type);
            }
        }
    }
}
