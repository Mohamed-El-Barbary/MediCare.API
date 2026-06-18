using CloudinaryDotNet.Core;
using Health.Domain.Contracts;
using Health.Domain.Entities.DoctorModule;
using Health.Services.Abstraction.IdentityModule;
using Health.Services.Specifications.DoctorSceduleSpecification;
using Health.Shared.CommonResponses;
using Microsoft.AspNetCore.Http.HttpResults;
using System;
using System.Collections.Generic;
using System.Text;

namespace Health.Services.ServicesImplementation.IdentityModule
{
    public class ProfileCompletionService : IProfileCompletionService
    {
        private readonly IUnitOfWork _unitOfWork;

        public ProfileCompletionService(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }
        public async Task<Result<int>> CalculateAndUpdateDoctorStatusAsync(string doctorId)
        {
            int total = 5;
            int completed = 0;
            var spec = new DoctorByUserIdSpec(doctorId);
            var doctor = await _unitOfWork.GetRepository<DoctorProfile, int>().GetByIdAsync(spec);
            if (doctor is null)
                return Result<int>.Fail(Error.NotFound("Doctor.NotFound", "Doctor With Id Not Found"));

            if (!string.IsNullOrWhiteSpace(doctor.PhoneClinc)) completed++;
            if (!string.IsNullOrWhiteSpace(doctor.Bio)) completed++;
            if (!string.IsNullOrWhiteSpace(doctor.SyndicateCardUrl)) completed++;
            if (!string.IsNullOrWhiteSpace(doctor.MedicalLicenseNumber)) completed++;
            if (!string.IsNullOrWhiteSpace(doctor.DoctorPictureUrl)) completed++;

            var percentage = (completed * 100) / total;

            doctor.VerificationStatus = percentage == 100
                ? VerificationStatus.PendingApproval
                : VerificationStatus.PendingProfileCompletion;

            return Result<int>.Ok(percentage);
        }
    }
}
