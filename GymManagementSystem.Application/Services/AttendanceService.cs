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

            var attendance = _mapper.Map<Attendance>(dto);
            attendance.CheckInTime = DateTime.UtcNow;

            await _unitOfWork.Attendances.AddAsync(attendance);
            await _unitOfWork.SaveChangesAsync();

            var savedAttendance = await _unitOfWork.Attendances.GetByIdAsync(attendance.Id);

            return _mapper.Map<AttendanceDto>(savedAttendance);
        }

        public async Task<List<AttendanceDto>> GetGymAttendanceAsync(int gymId)
        {
            var gym = await _unitOfWork.Gyms.GetByIdAsync(gymId);

            if (gym == null)
                throw new NotFoundException($"Gym with id = {gymId} not found");

            var history = await _unitOfWork.Attendances.GetByGymIdAsync(gymId);

            return _mapper.Map<List<AttendanceDto>>(history);
        }

        public async Task<List<AttendanceDto>> GetMemberAttendanceHistoryAsync(int memberId)
        {
            var history = await _unitOfWork.Attendances.GetByMemberIdAsync(memberId);

            return _mapper.Map<List<AttendanceDto>>(history);
        }
    }
}