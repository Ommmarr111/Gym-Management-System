using GymManagementSystem.Application.DTOs;
using GymManagementSystem.Domain.Entities;

namespace GymManagementSystem.Application.Interfaces
{
    public interface IGymService
    {
        Task<List<Gym>> GetAllGymsAsync();
        Task<Gym> GetGymByIdAsync(int id);
        Task<int> CreateGymAsync(Gym gym);

        Task UpdateGymAsync(int id, UpdateGymDto dto);
        Task DeleteGymAsync(int id);
    }
}