using GymManagementSystem.Api.Models;
using GymManagementSystem.Application.Exceptions;
using Microsoft.AspNetCore.Diagnostics;

public class ForbiddenExceptionHandler : IExceptionHandler
{
    public async ValueTask<bool> TryHandleAsync(
        HttpContext context,
        Exception exception,
        CancellationToken cancellationToken)
    {
        if (exception is not ForbiddenException)
            return false;

        context.Response.StatusCode = 403;

        await context.Response.WriteAsJsonAsync(new ErrorResponse
        {
            StatusCode = 403,
            Message = exception.Message
        }, cancellationToken);

        return true;
    }
}