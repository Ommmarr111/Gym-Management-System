using GymManagementSystem.Domain.Entities;

namespace GymManagementSystem.Application.Interfaces
{
    public interface IAttendanceRepository
    {

        Task<Attendance> AddAsync(Attendance attendance);
        Task<List<Attendance>> GetAllAsync();
        Task<List<Attendance>> GetByMemberIdAsync(int memberId);

        Task<List<Attendance>> GetByGymIdAsync(int gymId);

        Task<Attendance?> GetByIdAsync(int id);
        IQueryable<Attendance> GetAllAsQueryable();

    }
}