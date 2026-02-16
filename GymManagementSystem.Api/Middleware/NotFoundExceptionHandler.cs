// NotFoundExceptionHandler.cs
using GymManagementSystem.Api.Models;
using GymManagementSystem.Application.Exceptions;
using Microsoft.AspNetCore.Diagnostics;

public class NotFoundExceptionHandler : IExceptionHandler
{
    public async ValueTask<bool> TryHandleAsync(
        HttpContext context,
        Exception exception,
        CancellationToken cancellationToken)
    {
        if (exception is not NotFoundException)
            return false;

        context.Response.StatusCode = 404;

        await context.Response.WriteAsJsonAsync(new ErrorResponse
        {
            StatusCode = 404,
            Message = exception.Message
        }, cancellationToken);

        return true;
    }
}