using GymManagementSystem.Domain.Entities;

namespace GymManagementSystem.Application.Interfaces
{
    public interface IMembershipPlanRepository
    {
        Task<List<MembershipPlan>> GetAllAsync();
        Task<MembershipPlan?> GetByIdAsync(int id);

        Task<MembershipPlan> AddAsync(MembershipPlan plan);

        Task UpdateAsync(MembershipPlan plan);
        Task DeleteAsync(int id);

        Task<bool> HasActiveSubscriptionsAsync(int planId);

    }
}