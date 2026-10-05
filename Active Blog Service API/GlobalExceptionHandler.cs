using App.Application.Common.Exceptions;
using FluentValidation;
using Microsoft.AspNetCore.Diagnostics;


namespace Active_Blog_Service_API
{
    public class GlobalExceptionHandler(
            ILogger<GlobalExceptionHandler> logger) : IExceptionHandler
    {
        public async ValueTask<bool> TryHandleAsync(
            HttpContext httpContext,
            Exception exception,
            CancellationToken cancellationToken)
        {
            logger.LogError(
                exception,
                "An exception occurred: {Message}",
                exception.Message);

            var statusCode = exception switch
            {
                NotFoundException => StatusCodes.Status404NotFound,

                ForbiddenException => StatusCodes.Status403Forbidden,

                UnauthorizedAccessException => StatusCodes.Status401Unauthorized,

                ArgumentException or ValidationException => StatusCodes.Status400BadRequest,

                _ => StatusCodes.Status500InternalServerError
            };

            httpContext.Response.StatusCode = statusCode;

            await httpContext.Response.WriteAsJsonAsync(
                new
                {
                    statusCode,
                    message = statusCode == 500
                        ? "An unexpected error occurred."
                        : exception.Message
                },
                cancellationToken);

            return true;
        }
    }
}
