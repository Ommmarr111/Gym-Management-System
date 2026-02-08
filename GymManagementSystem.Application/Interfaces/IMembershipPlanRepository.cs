using GymManagementSystem.Domain.Entities;

namespace GymManagementSystem.Application.Interfaces
{
    public interface IMembershipPlanRepository
    {
        Task<List<MembershipPlan>> GetAllAsync();

        Task<MembershipPlan?> GetByIdAsync(int id);

        Task<int> AddAsync(MembershipPlan plan);

        Task UpdateAsync(MembershipPlan plan);

        Task DeleteAsync(int id);
    }
}
