namespace ChurchAttendance.Core.Exceptions;

/// <summary>
/// Base type for errors that should be reported to the API caller with a specific HTTP status.
/// They are translated to ProblemDetails by ApiExceptionHandler.
/// </summary>
public abstract class AppException : Exception
{
    protected AppException(string message, int statusCode) : base(message)
    {
        StatusCode = statusCode;
    }

    public int StatusCode { get; }
}

public sealed class BadRequestException(string message) : AppException(message, 400);

public sealed class ForbiddenException(string message) : AppException(message, 403);

public sealed class NotFoundException(string message) : AppException(message, 404);

public sealed class ConflictException(string message) : AppException(message, 409);

/// <summary>The request is well formed but a business rule prevents it (e.g. the day is closed).</summary>
public sealed class BusinessRuleException(string message) : AppException(message, 422);
