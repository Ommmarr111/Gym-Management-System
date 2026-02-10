using GymManagementSystem.Application.DTOs;

namespace GymManagementSystem.Application.Interfaces
{
    public interface ISubscriptionService
    {
        Task<SubscriptionDto> CreateSubscriptionAsync(CreateSubscriptionDto dto);
        Task<List<SubscriptionDto>> GetAllSubscriptionsAsync();
        Task<SubscriptionDto?> GetSubscriptionByIdAsync(int id);
        Task<bool> CancelSubscriptionAsync(int subscriptionId);
    }
}