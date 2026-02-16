using Health.Services.Exceptions;
using Microsoft.AspNetCore.Mvc;

namespace Health.Web.CustomMiddlewares
{
    public class ExceptionHandlerMiddleWare
    {
        private readonly RequestDelegate _next;
        private readonly ILogger<ExceptionHandlerMiddleWare> _logger;

        public ExceptionHandlerMiddleWare(RequestDelegate next, ILogger<ExceptionHandlerMiddleWare> logger)
        {
            _next = next;
            _logger = logger;
        }

        public async Task InvokeAsync(HttpContext httpContext)
        {
            try
            {
                await _next.Invoke(httpContext);

                await HandleNotFoundEndPointAsync(httpContext);

            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Something went wrong");

                var problem = new ProblemDetails()
                {
                    Title = "Error While Processing Http Request",
                    Detail = ex.Message,
                    Instance = httpContext.Request.Path,
                    Status = ex switch
                    {
                        NotFoundExceptions => StatusCodes.Status404NotFound,
                        _ => StatusCodes.Status500InternalServerError
                    },
                };

                httpContext.Response.StatusCode = problem.Status.Value;

                await httpContext.Response.WriteAsJsonAsync(problem);
            }
        }

        private static async Task HandleNotFoundEndPointAsync(HttpContext httpContext)
        {
            if (httpContext.Response.StatusCode == StatusCodes.Status404NotFound && !httpContext.Response.HasStarted)
            {
                var problem = new ProblemDetails()
                {
                    Title = "Error While Processing The Http Request , EndPoint Not Found",
                    Detail = $"EndPoint {httpContext.Request.Path} Not Found",
                    Status = StatusCodes.Status404NotFound,
                    Instance = httpContext.Request.Path
                };

                await httpContext.Response.WriteAsJsonAsync(problem);
            }
        }
    }
}
