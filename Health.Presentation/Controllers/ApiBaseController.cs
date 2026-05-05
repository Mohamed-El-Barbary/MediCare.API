using Health.Shared.CommonResponses;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.ModelBinding;
using System.Security.Claims;


namespace Health.Presentation.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class ApiBaseController : ControllerBase
    {

        // Handle Result Without Value 
        protected ActionResult HandleResult(Result result, string? successMessage = default)
        {

            if (result.IsSuccess)
                return successMessage is null
                      ? NoContent()
                      : Ok(new { message = successMessage });
            else
                return HandleProblem(result.Errors);
        }

        // Handle Result With Value
        protected ActionResult<TValue> HandleResult<TValue>(Result<TValue> result)
        {
            if (result.IsSuccess)
                return Ok(result.Value);
            else
                return HandleProblem(result.Errors);
        }

        protected void SetRefreshTokenCookie(string refreshToken, DateTime expires)
        {
            var cookieOptions = new CookieOptions
            {
                HttpOnly = true,
                Secure = true,
                SameSite = SameSiteMode.None,
                Path = "/",
                Expires = expires,
                IsEssential = true
            };

            Response.Cookies.Append("refreshToken", refreshToken, cookieOptions);
        }

        private ActionResult HandleProblem(IReadOnlyList<Error> errors)
        {
            // If No Errors Are Provided , Return 500 Error
            if (errors.Count == 0)
                return Problem(
                    statusCode: StatusCodes.Status500InternalServerError,
                    title: "An UnExpected Error Occurred"
                    );
            // If All Errors are Validation Errors , Handle Them As Validation Problem
            if (errors.All(x => x.ErrorTypes == ErrorTypes.Validation))
                return HandleValidationsErrors(errors);
            // If There's Only One Error , Handle It As A Single Error Problem
            return HandleSingleErrorProblem(errors[0]);
        }

        private ActionResult HandleSingleErrorProblem(Error error)
        {
            return Problem(
                title: error.Code,
                detail: error.Description,
                type: error.ErrorTypes.ToString(),
                statusCode: MapErrorTypeToStatusCode(error.ErrorTypes)
                );
        }

        private static int MapErrorTypeToStatusCode(ErrorTypes errorType)
            => errorType switch
            {
                ErrorTypes.NotFound => StatusCodes.Status404NotFound,
                ErrorTypes.Validation => StatusCodes.Status400BadRequest,
                ErrorTypes.Unauthorized => StatusCodes.Status401Unauthorized,
                ErrorTypes.InvalidCredentials => StatusCodes.Status401Unauthorized,
                ErrorTypes.Forbidden => StatusCodes.Status403Forbidden,
                ErrorTypes.Failure => StatusCodes.Status500InternalServerError,
                _ => StatusCodes.Status500InternalServerError
            };

        private ActionResult HandleValidationsErrors(IReadOnlyList<Error> errors)
        {
            var modelState = new ModelStateDictionary();

            foreach (var err in errors)
                modelState.AddModelError(err.Code, err.Description);

            return ValidationProblem(modelState);
        }

        protected string GetUserId()
        {
            var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;

            if (string.IsNullOrEmpty(userId))
                throw new UnauthorizedAccessException("Invalid token");

            return userId;
        }

        protected string GetUserRole()
        {
            var role = User.FindFirst(ClaimTypes.Role)?.Value;

            if (string.IsNullOrEmpty(role))
                throw new UnauthorizedAccessException("Invalid token");

            return role;
        }
    }
}
