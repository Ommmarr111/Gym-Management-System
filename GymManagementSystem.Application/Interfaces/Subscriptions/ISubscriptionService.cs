using GymManagementSystem.Application.DTOs;
using GymManagementSystem.Application.DTOs.Subscriptions;

namespace GymManagementSystem.Application.Interfaces
{
    public interface ISubscriptionService
    {
        Task<SubscriptionDto> CreateSubscriptionAsync(CreateSubscriptionDto dto);
        Task<PagedResult<SubscriptionDto>> GetAllSubscriptionsAsync(SubscriptionRequestParams subscriptionRequestParams);
        Task<SubscriptionDto> GetSubscriptionByIdAsync(int id);
        Task<SubscriptionDto> CancelSubscriptionAsync(int subscriptionId);
        Task<SubscriptionDto> FreezeSubscriptionAsync(int subscriptionId, FreezeSubscriptionDto dto);
        Task<SubscriptionDto> UnfreezeSubscriptionAsync(int subscriptionId);
    }
}