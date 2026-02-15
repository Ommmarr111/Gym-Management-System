using GymManagementSystem.Api.Models;
using GymManagementSystem.Application.Exceptions; // عشان يشوف الـ Exceptions بتاعتنا
using Microsoft.AspNetCore.Diagnostics;

namespace GymManagementSystem.Api.Middleware
{
    public class NotFoundExceptionHandler : IExceptionHandler
    {
        public async ValueTask<bool> TryHandleAsync(
            HttpContext context, Exception exception, CancellationToken ct)
        {
            if (exception is not GymNotFoundException &&
                exception is not MemberNotFoundException)
                return false;

            context.Response.StatusCode = 404;
            await context.Response.WriteAsJsonAsync(new ErrorResponse
            {
                StatusCode = 404,
                Message = exception.Message
            }, ct);

            return true;
        }
    }
}