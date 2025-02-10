using Microsoft.AspNetCore.Diagnostics;
using Microsoft.AspNetCore.Mvc;
using System.Net;

namespace OlineLibraryAPI.Error
{
    public class ExceptionHandler(ILogger<ExceptionHandler> logger) : IExceptionHandler
    {
        private readonly ILogger<ExceptionHandler> _logger = logger;

        public async ValueTask<bool> TryHandleAsync(HttpContext httpContext, Exception exception, CancellationToken cancellationToken)
        {
            string title;
            int status;

            switch (exception)
            {
                case BadHttpRequestException:
                    title = "Unexpected end of request content";
                    status = (int)HttpStatusCode.BadRequest;
                    break;
                case TimeoutException:
                    title = "A timeout occurred";
                    status = (int)HttpStatusCode.RequestTimeout;
                    break;

                default:
                    title = "An unexpected error occurred";
                    status = (int)HttpStatusCode.InternalServerError;
                    break;
            }

            _logger.LogError(exception, title);

            var problemDetails = new ProblemDetails
            {
                Status = status,
                Type = exception.GetType().Name,
                Title = title,
                Detail = exception.Message,
                Instance = $"{httpContext.Request.Method} {httpContext.Request.Path}"
            };
            await httpContext.Response.WriteAsJsonAsync(problemDetails, cancellationToken);
            return true;
        }
    }
}
