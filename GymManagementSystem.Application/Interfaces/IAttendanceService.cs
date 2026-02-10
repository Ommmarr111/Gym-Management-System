using GymManagementSystem.Application.DTOs;

namespace GymManagementSystem.Application.Interfaces
{
    public interface IAttendanceService
    {
        Task<AttendanceDto> CheckInAsync(CheckInDto dto);

        Task<List<AttendanceDto>> GetMemberAttendanceHistoryAsync(int memberId);
    }
}