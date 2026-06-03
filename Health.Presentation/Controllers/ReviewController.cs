using Health.Services.Abstraction.ReviewModule;
using Health.Shared;
using Health.Shared.CommonResponses;
using Health.Shared.DTOs.ReviewDTOs;
using Health.Shared.ReviewSpecParams;
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

        [Authorize(Roles = "Patient")]
        [HttpDelete("{reviewId}")]
        public async Task<ActionResult> DeleteReview(int reviewId)
        {
            string patientId = GetUserId();
            var result = await _review.DeleteReview(patientId, reviewId);
            return HandleResult(result , $"Review has been Deleted");
        }


        [Authorize(Roles = "Doctor")]
        [HttpGet("AvgRating")]
        public async Task<ActionResult<DoctorRatingDTO>> AvgRatingAndReviewsCount()
        {
            string DoctorId = GetUserId();
            var result = await _review.GetDoctorAverageRating(DoctorId);
            return HandleResult(result);
        }

        [Authorize(Roles = "Patient")]
        [HttpGet("Patient")]
        public async Task<ActionResult<PaginatedResult<ReviewResposeForPatinetDTO>>> PatientReviews([FromQuery] ReviewSpecParam reviewSpec)
        {
            var patientId = GetUserId();
            var result = await _review.PatientResponse(patientId , reviewSpec);
            return Ok(result);
        }
        [Authorize(Roles = "Doctor")]
        [HttpGet("Doctor")]
        public async Task<ActionResult<PaginatedResult<ReviewResposeForPatinetDTO>>> DoctorReviews([FromQuery] ReviewSpecParam reviewSpec)
        {
            var doctorId = GetUserId();
            var result = await _review.DoctorResponse(doctorId, reviewSpec);
            return Ok(result);
        }
    }
}
