using GymManagementSystem.Application.Exceptions;
using Microsoft.AspNetCore.Diagnostics;
using Microsoft.AspNetCore.Mvc;

namespace GymManagementSystem.Api.Middleware;

public class GlobalExceptionHandler : IExceptionHandler
{
    private readonly ILogger<GlobalExceptionHandler> _logger;

    public GlobalExceptionHandler(ILogger<GlobalExceptionHandler> logger)
    {
        _logger = logger;
    }

    public async ValueTask<bool> TryHandleAsync(HttpContext context, Exception exception, CancellationToken cancellationToken)
    {
        // 1. Log the error for the backend team
        _logger.LogError(exception, "API Exception Caught: {Message}", exception.Message);

        // 2. Map your 5 custom exceptions to standard HTTP Status Codes
        var statusCode = exception switch
        {
            ValidationException => StatusCodes.Status400BadRequest,
            UnauthorizedException => StatusCodes.Status401Unauthorized,
            ForbiddenException => StatusCodes.Status403Forbidden,
            NotFoundException => StatusCodes.Status404NotFound,
            BusinessRuleException => StatusCodes.Status409Conflict, // 409 Conflict or 422 Unprocessable Entity
            _ => StatusCodes.Status500InternalServerError
        };

        // 3. Build the standard ProblemDetails response
        var problemDetails = new ProblemDetails
        {
            Status = statusCode,
            Title = exception switch
            {
                ValidationException => "Validation failed",
                UnauthorizedException => "Authentication required",
                ForbiddenException => "Access denied",
                NotFoundException => "Resource not found",
                BusinessRuleException => "Business rule violation",
                _ => "An unexpected error occurred"
            },
            Detail = statusCode == StatusCodes.Status500InternalServerError ? null : exception.Message,
            Type = $"https://httpstatuses.com/{statusCode}"
        };

        // 4. Send the HTTP response
        context.Response.StatusCode = statusCode;
        await context.Response.WriteAsJsonAsync(problemDetails, cancellationToken);

        // 5. Tell .NET the error is handled
        return true;
    }
}