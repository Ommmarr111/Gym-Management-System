using GymManagementSystem.Application.DTOs;

namespace GymManagementSystem.Application.Interfaces
{
    public interface IMemberService
    {
        Task<List<MemberDto>> GetAllMembersAsync();
        Task<MemberDetailsDto> GetMemberByIdAsync(int id);
        Task<MemberDetailsDto> CreateMemberAsync(CreateMemberDto dto);
        Task UpdateMemberAsync(int id, CreateMemberDto dto);
        Task DeleteMemberAsync(int id);
    }
}