using AutoMapper;
using GymManagementSystem.Application.DTOs;
using GymManagementSystem.Application.DTOs.Attendance;
using GymManagementSystem.Application.Exceptions;
using GymManagementSystem.Application.Extensions;
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

        public async Task<PagedResult<AttendanceDto>> GetAttendanceHistoryAsync(AttendanceRequestParams parameters)
        {
            var query = _unitOfWork.Attendances.GetAllAsQueryable();

            // 1. Foreign Key Filters
            if (parameters.MemberId.HasValue)
            {
                query = query.Where(a => a.MemberId == parameters.MemberId.Value);
            }

            if (parameters.GymId.HasValue)
            {
                query = query.Where(a => a.GymId == parameters.GymId.Value);
            }

            // 2. Date Range Filters
            if (parameters.CheckInDateFrom.HasValue)
            {
                query = query.Where(a => a.CheckInTime >= parameters.CheckInDateFrom.Value);
            }

            if (parameters.CheckInDateTo.HasValue)
            {
                query = query.Where(a => a.CheckInTime <= parameters.CheckInDateTo.Value);
            }

            // 3. Sorting (Defaulting to Newest First)
            if (!string.IsNullOrWhiteSpace(parameters.SortBy))
            {
                query = parameters.SortBy.ToLower() switch
                {
                    "checkintime" => parameters.IsDescending ? query.OrderByDescending(a => a.CheckInTime) : query.OrderBy(a => a.CheckInTime),
                    "checkouttime" => parameters.IsDescending ? query.OrderByDescending(a => a.CheckOutTime) : query.OrderBy(a => a.CheckOutTime),
                    _ => query.OrderByDescending(a => a.Id)
                };
            }
            else
            {
                // For logs, always default to newest records first!
                query = query.OrderByDescending(a => a.Id);
            }

            // 4. Execute Engine
            var pagedEntities = await query.ToPagedResultAsync(parameters.PageNumber, parameters.PageSize);

            // 5. Map to DTOs
            var attendanceDtos = _mapper.Map<List<AttendanceDto>>(pagedEntities.Items);

            // 6. Package and Return
            return new PagedResult<AttendanceDto>(
                attendanceDtos,
                pagedEntities.TotalCount,
                pagedEntities.CurrentPage,
                pagedEntities.PageSize);
        }
    }
}