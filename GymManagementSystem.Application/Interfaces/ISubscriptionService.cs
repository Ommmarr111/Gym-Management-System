using GymManagementSystem.Application.DTOs;

namespace GymManagementSystem.Application.Interfaces
{
    public interface ISubscriptionService
    {
        Task<SubscriptionDto> CreateSubscriptionAsync(CreateSubscriptionDto dto);
        Task<List<SubscriptionDto>> GetAllSubscriptionsAsync();
        Task<SubscriptionDto> GetSubscriptionByIdAsync(int id);
        Task CancelSubscriptionAsync(int subscriptionId);
        Task<List<SubscriptionDto>> GetSubscriptionsByMemberIdAsync(int memberId);

        Task FreezeSubscriptionAsync(int subscriptionId, FreezeSubscriptionDto dto);
        Task UnfreezeSubscriptionAsync(int subscriptionId);
    }
}