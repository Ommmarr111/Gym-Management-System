using GymManagementSystem.Domain.Entities;

namespace GymManagementSystem.Application.Interfaces
{
    public interface IGymRepository
    {
        Task<List<Gym>> GetAllAsync();
        Task<Gym?> GetByIdAsync(int id);
        Task<int> AddAsync(Gym gym);

        Task UpdateAsync(Gym gym);
        Task DeleteAsync(int id);
    }
}