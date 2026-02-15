using GymManagementSystem.Api.Models;
using GymManagementSystem.Application.Exceptions;
using Microsoft.AspNetCore.Diagnostics;

namespace GymManagementSystem.Api.Middleware
{
    public class BusinessRuleExceptionHandler : IExceptionHandler
    {
        public async ValueTask<bool> TryHandleAsync(
            HttpContext context, Exception exception, CancellationToken ct)
        {
            // بنمسك هنا اشتراك منتهي أو أي قاعدة بيزنس تانية
            if (exception is not SubscriptionExpiredException)//&&
                                                              //exception is not GymAccessDeniedException )
            {
                return false;
            }

            context.Response.StatusCode = 403;

            await context.Response.WriteAsJsonAsync(new ErrorResponse
            {
                StatusCode = 403,
                Message = exception.Message // "Subscription expired on..."
            }, ct);

            return true;
        }
    }
}