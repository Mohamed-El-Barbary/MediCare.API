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
        public async Task<Result<ReviewResposeDTO>> CreateReviewOnDoctor(string PatientUserId, CreateReviewDto createReviewDto)
        {
            var patientId = await getPatientId(PatientUserId);
            if (patientId is null)
                return Result<ReviewResposeDTO>.Fail(Error.Unauthorized("Patient.UnAuthorize", "Patient With This Id Not Found"));

            var Appointment = await GetAppointmentById(createReviewDto.AppointmentId);
            if(Appointment is null)
                return Result<ReviewResposeDTO>.Fail(Error.NotFound("Appointment.NotFound", "Appointment With This Id Not Found"));

            if (patientId != Appointment.PatientProfileId)
                return Result<ReviewResposeDTO>.Fail(
                    Error.Unauthorized(
                        "Patient.Unauthorized", 
                    "The patient is not allowed to rate this doctor because they did not book appointment with him."
                ));

            if(Appointment.Status != AppointmentStatus.Completed)
                return Result<ReviewResposeDTO>.Fail(
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

                var response = _mapper.Map<ReviewResposeDTO>(review);
                return Result<ReviewResposeDTO>.Ok(response);
            }

            return Result<ReviewResposeDTO>.Fail(Error.Failure("Review.Failer" , "Failure When Create Rview"));
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
