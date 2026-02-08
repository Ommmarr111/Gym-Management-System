using GymManagementSystem.Application.DTOs;

namespace GymManagementSystem.Application.Interfaces
{
    public interface IMembershipPlanService
    {
        Task<List<MembershipPlanDto>> GetAllPlansAsync();

        Task<MembershipPlanDto?> GetPlanByIdAsync(int id);

        Task<MembershipPlanDto> CreatePlanAsync(CreateMembershipPlanDto planDto);
        Task<bool> UpdatePlanAsync(int id, CreateMembershipPlanDto planDto);
        Task<bool> DeletePlanAsync(int id);
    }
}