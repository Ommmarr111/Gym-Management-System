using GymManagementSystem.Domain.Entities;

namespace GymManagementSystem.Application.Interfaces
{
    public interface ISubscriptionRepository
    {
        Task<List<Subscription>> GetAllAsync();
        Task<Subscription?> GetByIdAsync(int id);
        Task<Subscription> AddAsync(Subscription subscription);
        Task UpdateStatusAsync(int subscriptionId, string newStatus);
        Task<List<Subscription>> GetByMemberIdAsync(int memberId);
        Task UpdateAsync(Subscription subscription);

        Task<Subscription?> GetActiveSubscriptionAsync(int memberId, int membershipPlanId);

        IQueryable<Subscription> GetAllAsQueryable();

        Task<List<Subscription>> GetOverdueActiveSubscriptionsAsync(DateTime asOf);

    }
}