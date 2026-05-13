using AutoMapper;
using Health.Domain.Contracts;
using Health.Domain.Entities.AppointmentModule;
using Health.Domain.Entities.PatientModule;
using Health.Domain.Entities.ReviewModule;
using Health.Services.Abstraction.ReviewModule;
using Health.Services.Specifications.PatientSpecification;
using Health.Shared.CommonResponses;
using Health.Shared.DTOs.ReviewDTOs;
using System;
using System.Collections.Generic;
using System.Text;

namespace Health.Services.ServicesImplementation.ReviewModuleService
{
    public class ReviewService : IReview
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IMapper _mapper;

        public ReviewService(IUnitOfWork unitOfWork , IMapper mapper)
        {
            _unitOfWork = unitOfWork;
            _mapper = mapper;
        }
        public async Task<Result<ReviewResposeForPatinetDTO>> CreateReviewOnDoctor(string PatientUserId, CreateReviewDto createReviewDto)
        {
            var patientId = await getPatientId(PatientUserId);
            if (patientId is null)
                return Result<ReviewResposeForPatinetDTO>.Fail(Error.Unauthorized("Patient.UnAuthorize", "Patient With This Id Not Found"));

            var Appointment = await GetAppointmentById(createReviewDto.AppointmentId);
            if(Appointment is null)
                return Result<ReviewResposeForPatinetDTO>.Fail(Error.NotFound("Appointment.NotFound", "Appointment With This Id Not Found"));

            if (patientId != Appointment.PatientProfileId)
                return Result<ReviewResposeForPatinetDTO>.Fail(
                    Error.Unauthorized(
                        "Patient.Unauthorized", 
                    "The patient is not allowed to rate this doctor because they did not book appointment with him."
                ));

            if(Appointment.Status != AppointmentStatus.Completed)
                return Result<ReviewResposeForPatinetDTO>.Fail(
                    Error.InvalidCredentials(
                        "Patient.NotAllow",
                    "The patient is not allowed to rate this doctor because they did not complete an appointment with him."
                ));

            
            var review = _mapper.Map<Review>(createReviewDto);
            review.PatientId = (int)patientId;
            review.doctorId = Appointment.DoctorProfileId;
            review.CreatedAt = DateTime.UtcNow;

            await _unitOfWork.GetRepository<Review, int>().AddAsync(review);

            if (await _unitOfWork.SaveChanges() > 0)
            {

                var response = _mapper.Map<ReviewResposeForPatinetDTO>(review);
                return Result<ReviewResposeForPatinetDTO>.Ok(response);
            }

            return Result<ReviewResposeForPatinetDTO>.Fail(Error.Failure("Review.Failer" , "Failure When Create Rview"));
        }

        public async Task<Result<ReviewResposeForPatinetDTO>> UpdateReview(string PatientUserId, int reviewId, UpdateReviewDto updateReviewDto)
        {
            var patientId = await getPatientId(PatientUserId);
            if (patientId is null)
                return Error.Unauthorized("Patient.UnAuthorize", "Patient With This Id Not Found");

            var review = await _unitOfWork.GetRepository<Review, int>().GetByIdAsync(reviewId);
            if(review is null)
                return Error.NotFound("Review.", "Review With This Id Not Found");

            if (review.PatientId != patientId)
                return Error.Unauthorized("Patient.NotAllow", "Patient Not allow To Update This Review");

            var hoursSinceCreation = DateTime.UtcNow - review.CreatedAt;
            if (hoursSinceCreation.TotalHours > 24)
                return Error.Failure("Update.NotAllowed" , "You can only edit review within 24 hours");

            review.Rating = updateReviewDto.Rating;
            if(updateReviewDto.comment is not null)
                review.Comment = updateReviewDto.comment;
            review.UpdateAt = DateTime.UtcNow;

            _unitOfWork.GetRepository<Review,int>().Update(review);

            if(await _unitOfWork.SaveChanges() > 0)
            {
                var response = _mapper.Map<ReviewResposeForPatinetDTO>(review);
                return Result<ReviewResposeForPatinetDTO>.Ok(response);
            }

            return Error.Failure("Review.Failure" , "Review Failure To Update");
        }


        #region HelperMethod
        private async Task<int?> getPatientId(string PatientUserId)
        {
            var spec = new PatientByIdWithoutIncludes(PatientUserId);
            var patient = await _unitOfWork.GetRepository<PatientProfile, int>().GetByIdAsync(spec);
            if (patient is null)
                return null;
            else
                return patient.Id;
        }

        private async Task<Appointment?> GetAppointmentById(int AppointmentId)
        {
            var Appointment = await _unitOfWork.GetRepository<Appointment, int>().GetByIdAsync(AppointmentId);
            if (Appointment is null)
                return null;
            else
                return Appointment;
        }

        #endregion


    }
}
