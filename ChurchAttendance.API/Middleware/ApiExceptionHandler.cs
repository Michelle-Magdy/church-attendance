using ChurchAttendance.Core.Exceptions;
using Microsoft.AspNetCore.Diagnostics;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.WebUtilities;

namespace ChurchAttendance.API.Middleware;

/// <summary>Turns exceptions thrown by the services into ProblemDetails responses.</summary>
public sealed class ApiExceptionHandler : IExceptionHandler
{
    private readonly IProblemDetailsService _problemDetails;
    private readonly ILogger<ApiExceptionHandler> _logger;

    public ApiExceptionHandler(IProblemDetailsService problemDetails, ILogger<ApiExceptionHandler> logger)
    {
        _problemDetails = problemDetails;
        _logger = logger;
    }

    public async ValueTask<bool> TryHandleAsync(HttpContext httpContext, Exception exception, CancellationToken cancellationToken)
    {
        int status;
        string detail;

        switch (exception)
        {
            case AppException app:
                status = app.StatusCode;
                detail = app.Message;
                break;
            case UnauthorizedAccessException unauthorized:
                status = StatusCodes.Status401Unauthorized;
                detail = unauthorized.Message;
                break;
            default:
                status = StatusCodes.Status500InternalServerError;
                detail = "An unexpected error occurred.";
                _logger.LogError(exception, "Unhandled exception");
                break;
        }

        httpContext.Response.StatusCode = status;

        return await _problemDetails.TryWriteAsync(new ProblemDetailsContext
        {
            HttpContext = httpContext,
            Exception = exception,
            ProblemDetails = new ProblemDetails
            {
                Status = status,
                Title = ReasonPhrases.GetReasonPhrase(status),
                Detail = detail
            }
        });
    }
}
