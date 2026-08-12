using GymManagementSystem.Application.DTOs;

namespace GymManagementSystem.Application.Interfaces
{
    public interface IGymService
    {
        Task<List<GymDto>> GetAllGymsAsync();
        Task<GymDto> GetGymByIdAsync(int id);
        Task<GymDto> CreateGymAsync(CreateGymDto dto);
        Task UpdateGymAsync(int id, UpdateGymDto dto);
        Task DeleteGymAsync(int id);
    }
}