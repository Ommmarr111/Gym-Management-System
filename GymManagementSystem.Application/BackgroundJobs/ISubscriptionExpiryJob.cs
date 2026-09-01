namespace GymManagementSystem.Application.BackgroundJobs
{
    public interface ISubscriptionExpiryJob
    {
        Task ExpireOverdueSubscriptionsAsync();
    }
}
