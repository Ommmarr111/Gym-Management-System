using GymManagementSystem.Api.Models;
using Microsoft.AspNetCore.Diagnostics;

public class GlobalExceptionHandler : IExceptionHandler
{
    public async ValueTask<bool> TryHandleAsync(
        HttpContext context,
        Exception exception,
        CancellationToken cancellationToken)
    {
        context.Response.StatusCode = 500;

        await context.Response.WriteAsJsonAsync(new ErrorResponse
        {
            StatusCode = StatusCodes.Status500InternalServerError,
            Message = "Internal Server Error. Please try again later."
        }, cancellationToken);

        return true;
    }
}