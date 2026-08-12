using GymManagementSystem.Application.DTOs;

namespace GymManagementSystem.Application.Interfaces
{
    public interface IMembershipPlanService
    {
        Task<List<MembershipPlanDto>> GetAllPlansAsync();

        Task<MembershipPlanDto> GetPlanByIdAsync(int id);

        Task<List<MembershipPlanDto>> GetPlansByGymIdAsync(int gymId);

        Task<MembershipPlanDto> CreatePlanAsync(CreateMembershipPlanDto planDto);
        Task UpdatePlanAsync(int id, UpdateMembershipPlanDto planDto);
        Task DeletePlanAsync(int id);
    }
}