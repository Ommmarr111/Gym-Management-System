using GymManagementSystem.Api.Models;
using Microsoft.AspNetCore.Diagnostics;
using System.ComponentModel.DataAnnotations;

public class ValidationExceptionHandler : IExceptionHandler
{
    public async ValueTask<bool> TryHandleAsync(
        HttpContext context,
        Exception exception,
        CancellationToken cancellationToken)
    {
        if (exception is not ValidationException)
            return false;

        context.Response.StatusCode = 400; // Bad Request

        await context.Response.WriteAsJsonAsync(new ErrorResponse
        {
            StatusCode = 400,
            Message = exception.Message
        }, cancellationToken);

        return true;
    }
}