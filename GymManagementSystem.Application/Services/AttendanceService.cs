using GymManagementSystem.Application.DTOs;
using GymManagementSystem.Application.Interfaces;
using GymManagementSystem.Domain.Entities;

namespace GymManagementSystem.Application.Services
{
    public class AttendanceService : IAttendanceService
    {
        private readonly IAttendanceRepository _attendanceRepo;
        private readonly ISubscriptionRepository _subscriptionRepo;
        private readonly IGymRepository _gymRepo;

        public AttendanceService(
            IAttendanceRepository attendanceRepo,
            ISubscriptionRepository subscriptionRepo,
            IGymRepository gymRepo)
        {
            _attendanceRepo = attendanceRepo;
            _subscriptionRepo = subscriptionRepo;
            _gymRepo = gymRepo;
        }

        public async Task<AttendanceDto> CheckInAsync(CheckInDto dto)
        {
            var currentGym = await _gymRepo.GetByIdAsync(dto.GymId);

            if (currentGym == null || currentGym.IsDeleted)
            {
                throw new Exception("This Gym is currently closed or does not exist. ⛔");
            }

            var memberSubs = await _subscriptionRepo.GetByMemberIdAsync(dto.MemberId);

            var validSubscription = memberSubs.FirstOrDefault(s =>
                s.Status == "Active" &&
                s.EndDate.Date >= DateTime.UtcNow.Date &&
                s.MembershipPlan.GymId == dto.GymId
            );

            if (validSubscription == null)
            {
                throw new Exception("Access Denied: No active subscription found for this gym!");
            }

            var attendance = new Attendance
            {
                MemberId = dto.MemberId,
                GymId = dto.GymId,
                CheckInTime = DateTime.UtcNow
            };

            await _attendanceRepo.AddAsync(attendance);

            var gym = await _gymRepo.GetByIdAsync(dto.GymId);

            return new AttendanceDto
            {
                Id = attendance.Id,
                MemberName = validSubscription.Member != null
                    ? $"{validSubscription.Member.FirstName} {validSubscription.Member.LastName}"
                    : "Valued Member",
                GymName = gym != null ? gym.Name : "Unknown Gym",
                CheckInTime = attendance.CheckInTime.ToString("yyyy-MM-dd hh:mm tt") // تنسيق: 2026-02-10 07:30 PM
            };
        }

        public async Task<List<AttendanceDto>> GetMemberAttendanceHistoryAsync(int memberId)
        {
            var history = await _attendanceRepo.GetByMemberIdAsync(memberId);

            return history.Select(a => new AttendanceDto
            {
                Id = a.Id,
                MemberName = a.Member != null ? $"{a.Member.FirstName} {a.Member.LastName}" : "Unknown",
                GymName = a.Gym != null ? a.Gym.Name : "Unknown",
                CheckInTime = a.CheckInTime.ToString("yyyy-MM-dd hh:mm tt")
            }).ToList();
        }
    }
}