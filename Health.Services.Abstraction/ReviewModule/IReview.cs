using Health.Shared.CommonResponses;
using Health.Shared.DTOs.ReviewDTOs;
using System;
using System.Collections.Generic;
using System.Text;

namespace Health.Services.Abstraction.ReviewModule
{
    public interface IReview
    {
        Task<Result<ReviewResposeDTO>> CreateReviewOnDoctor(string PatientUserId , CreateReviewDto createReviewDto);
    }
}
