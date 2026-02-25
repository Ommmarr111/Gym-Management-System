using AutoMapper;
using GymManagementSystem.Application.DTOs;
using GymManagementSystem.Application.Exceptions;
using GymManagementSystem.Application.Interfaces;
using GymManagementSystem.Domain.Entities;

namespace GymManagementSystem.Application.Services
{
    public class AttendanceService : IAttendanceService
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IMapper _mapper;

        public AttendanceService(IUnitOfWork unitOfWork, IMapper mapper)
        {
            _unitOfWork = unitOfWork;
            _mapper = mapper;
        }
        public async Task<AttendanceDto> CheckInAsync(CheckInDto dto)
        {
            var currentGym = await _unitOfWork.Gyms.GetByIdAsync(dto.GymId);

            if (currentGym == null || currentGym.IsDeleted)
                throw new NotFoundException($"Gym with id = {dto.GymId} does not exist or is closed");

            var memberSubs = await _unitOfWork.Subscriptions.GetByMemberIdAsync(dto.MemberId);

            var validSubscription = memberSubs.FirstOrDefault(s =>
                s.Status == "Active" &&
                s.EndDate.Date >= DateTime.UtcNow.Date &&
                s.MembershipPlan.GymId == dto.GymId
            );

            if (validSubscription == null)
                throw new ForbiddenException("Access denied: no active subscription found for this gym");

            var attendance = new Attendance
            {
                MemberId = dto.MemberId,
                GymId = dto.GymId,
                CheckInTime = DateTime.UtcNow
            };

            await _unitOfWork.Attendances.AddAsync(attendance);
            await _unitOfWork.SaveChangesAsync();

            return new AttendanceDto
            {
                Id = attendance.Id,
                MemberName = validSubscription.Member != null
                    ? $"{validSubscription.Member.FirstName} {validSubscription.Member.LastName}"
                    : "Valued Member",
                GymName = currentGym.Name,
                CheckInTime = attendance.CheckInTime.ToString("yyyy-MM-dd hh:mm tt")
            };
        }
        public async Task<List<AttendanceDto>> GetGymAttendanceAsync(int gymId)
        {
            var gym = await _unitOfWork.Gyms.GetByIdAsync(gymId);

            if (gym == null)
                throw new NotFoundException($"Gym with id = {gymId} not found");

            var history = await _unitOfWork.Attendances.GetByGymIdAsync(gymId);

            return history.Select(a => new AttendanceDto
            {
                Id = a.Id,
                MemberName = a.Member != null
                    ? $"{a.Member.FirstName} {a.Member.LastName}"
                    : "Unknown",
                GymName = gym.Name,
                CheckInTime = a.CheckInTime.ToString("yyyy-MM-dd hh:mm tt")
            }).ToList();
        }

        public async Task<List<AttendanceDto>> GetMemberAttendanceHistoryAsync(int memberId)
        {
            var history = await _unitOfWork.Attendances.GetByMemberIdAsync(memberId);

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