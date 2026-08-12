using GymManagementSystem.Application.DTOs.Shared;

namespace GymManagementSystem.Application.DTOs.Attendance;

public class AttendanceRequestParams : PaginationParams
{
    public int? MemberId { get; set; }
    public int? GymId { get; set; }

    // Using From/To for date ranges is the standard for log querying
    public DateTime? CheckInDateFrom { get; set; }
    public DateTime? CheckInDateTo { get; set; }
}