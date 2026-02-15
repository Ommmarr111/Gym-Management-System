using GymManagementSystem.Api.Models;
using Microsoft.AspNetCore.Diagnostics;

namespace GymManagementSystem.Api.Middleware
{
    public class GlobalExceptionHandler : IExceptionHandler
    {
        public async ValueTask<bool> TryHandleAsync(
            HttpContext context, Exception exception, CancellationToken ct)
        {
            context.Response.StatusCode = 500;

            await context.Response.WriteAsJsonAsync(new ErrorResponse
            {
                StatusCode = 500,
                Message = "Internal Server Error. Please try again later."
            }, ct);

            return true;
        }
    }
}