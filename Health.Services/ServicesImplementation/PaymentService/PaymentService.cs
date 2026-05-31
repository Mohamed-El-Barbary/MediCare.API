using CloudinaryDotNet.Actions;
using Health.Domain.Contracts;
using Health.Domain.Entities.AppointmentModule;
using Health.Services.Abstraction.PaymentServiceAbstraction;
using Health.Services.Specifications.AppointmentSpecification;
using Health.Shared.CommonResponses;
using Health.Shared.DTOs.AppointmentDTOs;
using Health.Shared.DTOs.PaymentDTOs;
using Health.Shared.ParamsForFilterationPatientAppointment;
using Microsoft.Extensions.Configuration;
using SendGrid.Helpers.Mail;
using Stripe;
using System;
using System.Collections.Generic;
using System.Text;
using Error = Health.Shared.CommonResponses.Error;

namespace Health.Services.ServicesImplementation.PaymentService
{
    public class PaymentService : IPaymentService
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IConfiguration _configuration;

        public PaymentService(IUnitOfWork unitOfWork , IConfiguration configuration)
        {
            _unitOfWork = unitOfWork;
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

            if (
                appointment.Status == AppointmentStatus.AppointmentCancelled ||
                appointment.Status == AppointmentStatus.AppointmentCompleted ||
                appointment.Status == AppointmentStatus.AppointmentConfirmed
               )
                return Error.Validation("Cannot pay for cancelled Or Completed Or Confirmed appointment");

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
    }
}
