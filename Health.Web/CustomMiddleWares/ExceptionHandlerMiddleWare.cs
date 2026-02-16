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
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Something went wrong");

                var problem = new ProblemDetails()
                {
                    Title = "Error While Processing Http Request",
                    Detail = ex.Message,
                    Instance = httpContext.Request.Path,
                    Status = StatusCodes.Status500InternalServerError,
                };

                await httpContext.Response.WriteAsJsonAsync(problem);
            }
        }

    }
}
