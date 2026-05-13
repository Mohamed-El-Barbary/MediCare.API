using Health.Services.Abstraction.ReviewModule;
using Health.Shared.DTOs.ReviewDTOs;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Text;

namespace Health.Presentation.Controllers
{
    public class ReviewController : ApiBaseController
    {
        private readonly IReview _review;

        public ReviewController(IReview review)
        {
            _review = review;
        }


        [Authorize(Roles = "Patient")]
        [HttpPost]
        public async Task<ActionResult<ReviewResposeForPatinetDTO>> CreateReview([FromBody] CreateReviewDto dto)
        {
            var userId = GetUserId();
            var result = await _review.CreateReviewOnDoctor(userId , dto);
            return HandleResult(result);
        }


        [Authorize(Roles = "Patient")]
        [HttpPut("{reviewId}")]
        public async Task<ActionResult<ReviewResposeForPatinetDTO>> UpdateReview(int reviewId, [FromBody] UpdateReviewDto dto)
        {
            var userId = GetUserId();
            var result = await _review.UpdateReview(userId , reviewId , dto);
            return HandleResult(result);
        }
    }
}
