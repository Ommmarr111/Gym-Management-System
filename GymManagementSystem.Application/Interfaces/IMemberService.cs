using GymManagementSystem.Application.DTOs;
using GymManagementSystem.Application.DTOs.Members;

namespace GymManagementSystem.Application.Interfaces
{
    public interface IMemberService
    {

        Task<PagedResult<MemberDto>> GetAllMembersAsync(MemberRequestParams parameters);
        Task<MemberDetailsDto> GetMemberByIdAsync(int id);
        Task<MemberDetailsDto> CreateMemberAsync(CreateMemberDto dto);
        Task UpdateMemberAsync(int id, CreateMemberDto dto);
        Task DeleteMemberAsync(int id);
    }
}