using GymManagementSystem.Application.DTOs;

namespace GymManagementSystem.Application.Interfaces
{
    public interface IMemberService
    {
        Task<List<MemberDto>> GetAllMembersAsync();
        Task<MemberDto?> GetMemberByIdAsync(int id);
        Task<MemberDto> CreateMemberAsync(CreateMemberDto memberDto);
        Task<bool> UpdateMemberAsync(int id, CreateMemberDto memberDto);
        Task<bool> DeleteMemberAsync(int id);
    }
}