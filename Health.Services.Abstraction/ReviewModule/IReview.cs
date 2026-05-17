using Health.Shared;
using Health.Shared.CommonResponses;
using Health.Shared.DTOs.ReviewDTOs;
using Health.Shared.ReviewSpecParams;
using System;
using System.Collections.Generic;
using System.Text;

namespace Health.Services.Abstraction.ReviewModule
{
    public interface IReview
    {
        Task<Result<ReviewResposeForPatinetDTO>> CreateReviewOnDoctor(string PatientUserId , CreateReviewDto createReviewDto);
        Task<Result<ReviewResposeForPatinetDTO>> UpdateReview(string PatientUserId, int reviewId , UpdateReviewDto createReviewDto);
        Task<Result> DeleteReview(string PatientUserId , int reviewId);
        Task<Result<DoctorRatingDTO>> GetDoctorAverageRating(string DoctorUserId);
        Task<PaginatedResult<ReviewResposeForPatinetDTO>> PatientResponse(string patientUserId, ReviewSpecParam reviewParams);
    }
}
