using Health.Shared.CommonResponses;
using Health.Shared.DTOs.ReviewDTOs;
using System;
using System.Collections.Generic;
using System.Text;

namespace Health.Services.Abstraction.ReviewModule
{
    public interface IReview
    {
        Task<Result<ReviewResposeForPatinetDTO>> CreateReviewOnDoctor(string PatientUserId , CreateReviewDto createReviewDto);
        Task<Result<ReviewResposeForPatinetDTO>> UpdateReview(string PatientUserId, int reviewId , UpdateReviewDto createReviewDto);
    }
}
