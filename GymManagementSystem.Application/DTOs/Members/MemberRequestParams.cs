using GymManagementSystem.Application.DTOs.Shared;

namespace GymManagementSystem.Application.DTOs.Members;

public class MemberRequestParams : PaginationParams
{
    public string? SearchTerm { get; set; }
    public int? GymId { get; set; }
}