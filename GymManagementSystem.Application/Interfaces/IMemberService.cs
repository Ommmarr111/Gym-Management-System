using GymManagementSystem.Application.DTOs;

namespace GymManagementSystem.Application.Interfaces
{
    public interface IMemberService
    {
        Task<List<MemberDto>> GetAllMembersAsync();

        Task<MemberDto> CreateMemberAsync(CreateMemberDto memberDto);
        Task<MemberDto> GetMemberByIdAsync(int id);
        Task UpdateMemberAsync(int id, CreateMemberDto dto);
        Task DeleteMemberAsync(int id);
    }
}