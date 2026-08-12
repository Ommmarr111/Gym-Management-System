using GymManagementSystem.Application.DTOs;
using GymManagementSystem.Application.DTOs.Attendance;

namespace GymManagementSystem.Application.Interfaces
{
    public interface IAttendanceService
    {

        Task<PagedResult<AttendanceDto>> GetAttendanceHistoryAsync(AttendanceRequestParams parameters);
        Task<AttendanceDto> CheckInAsync(CheckInDto dto);

        Task<List<AttendanceDto>> GetMemberAttendanceHistoryAsync(int memberId);

        Task<List<AttendanceDto>> GetGymAttendanceAsync(int gymId);
    }
}