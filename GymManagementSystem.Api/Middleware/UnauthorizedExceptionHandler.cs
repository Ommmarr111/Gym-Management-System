using GymManagementSystem.Api.Models;
using Microsoft.AspNetCore.Diagnostics;

public class UnauthorizedExceptionHandler : IExceptionHandler
{
    public async ValueTask<bool> TryHandleAsync(
        HttpContext context,
        Exception exception,
        CancellationToken cancellationToken)
    {
        if (exception is not UnauthorizedException)
            return false;

        context.Response.StatusCode = StatusCodes.Status401Unauthorized;

        await context.Response.WriteAsJsonAsync(new ErrorResponse
        {
            StatusCode = StatusCodes.Status401Unauthorized,
            Message = exception.Message
        }, cancellationToken);

        return true;
    }
}