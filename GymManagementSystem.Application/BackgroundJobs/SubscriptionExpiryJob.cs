using GymManagementSystem.Application.Interfaces;
using Microsoft.Extensions.Logging;

namespace GymManagementSystem.Application.BackgroundJobs
{
    public class SubscriptionExpiryJob : ISubscriptionExpiryJob
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly ILogger<SubscriptionExpiryJob> _logger;

        public SubscriptionExpiryJob(IUnitOfWork unitOfWork, ILogger<SubscriptionExpiryJob> logger)
        {
            _unitOfWork = unitOfWork;
            _logger = logger;
        }

        public async Task ExpireOverdueSubscriptionsAsync()
        {
            var expiredSubscriptions = await _unitOfWork.Subscriptions
                .GetOverdueActiveSubscriptionsAsync(DateTime.UtcNow);

            if (!expiredSubscriptions.Any())
            {
                _logger.LogInformation("No overdue subscriptions found.");
                return;
            }

            foreach (var subscription in expiredSubscriptions)
                subscription.Status = "Expired";

            await _unitOfWork.SaveChangesAsync();

            _logger.LogInformation($"Expired {expiredSubscriptions.Count} overdue subscriptions.", expiredSubscriptions.Count);
        }
    }
}
