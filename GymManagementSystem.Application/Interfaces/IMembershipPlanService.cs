using GymManagementSystem.Application.DTOs;

namespace GymManagementSystem.Application.Interfaces
{
    public interface IMembershipPlanService
    {
        Task<List<MembershipPlanDto>> GetAllPlansAsync();

        Task<MembershipPlanDto?> GetPlanByIdAsync(int id);

        Task<MembershipPlanDto> CreatePlanAsync(CreateMembershipPlanDto planDto);
        Task UpdatePlanAsync(int id, UpdateMembershipPlanDto planDto);
        Task DeletePlanAsync(int id);
    }
}