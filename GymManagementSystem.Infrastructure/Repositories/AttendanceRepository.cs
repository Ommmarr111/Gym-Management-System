using GymManagementSystem.Application.Interfaces;
using GymManagementSystem.Domain.Entities;
using GymManagementSystem.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;

namespace GymManagementSystem.Infrastructure.Repositories
{
    public class AttendanceRepository : IAttendanceRepository
    {
        private readonly ApplicationDbContext _context;

        public AttendanceRepository(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<Attendance> AddAsync(Attendance attendance)
        {
            await _context.Attendances.AddAsync(attendance);
            return attendance;
        }

        public async Task<List<Attendance>> GetAllAsync()
        {
            return await _context.Attendances
                .Include(a => a.Member)
                .Include(a => a.Gym)
                .OrderByDescending(a => a.CheckInTime)
                .ToListAsync();
        }
        public async Task<List<Attendance>> GetByGymIdAsync(int gymId)
        {
            return await _context.Attendances
                .Include(a => a.Member)
                .Include(a => a.Gym)
                .Where(a => a.GymId == gymId)
                .OrderByDescending(a => a.CheckInTime)
                .ToListAsync();
        }

        public async Task<List<Attendance>> GetByMemberIdAsync(int memberId)
        {
            return await _context.Attendances
                .IgnoreQueryFilters()
                .Include(a => a.Gym)
                .Include(a => a.Member)
                .Where(a => a.MemberId == memberId)
                .OrderByDescending(a => a.CheckInTime)
                .ToListAsync();
        }
    }
}