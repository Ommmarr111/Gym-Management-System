using GymManagementSystem.Domain.Entities;

namespace GymManagementSystem.Application.Interfaces
{
    public interface IMembershipPlanService
    {
        Task<List<MembershipPlan>> GetAllPlansAsync();
        Task<MembershipPlan?> GetPlanByIdAsync(int id);
        Task<int> CreatePlanAsync(MembershipPlan plan);

    }
}
