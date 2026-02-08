using GymManagementSystem.Domain.Entities;

namespace GymManagementSystem.Application.Interfaces
{
    public interface IGymService
    {
        Task<List<Gym>> GetAllGymsAsync();
        Task<Gym?> GetGymByIdAsync(int id);
        Task<int> CreateGymAsync(Gym gym);
    }
}