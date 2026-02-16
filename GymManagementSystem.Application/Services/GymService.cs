using GymManagementSystem.Application.DTOs;
using GymManagementSystem.Application.Exceptions;
using GymManagementSystem.Application.Interfaces;
using GymManagementSystem.Domain.Entities;

namespace GymManagementSystem.Application.Services
{
    public class GymService : IGymService
    {
        private readonly IGymRepository _repository;

        public GymService(IGymRepository repository)
        {
            _repository = repository;
        }

        public async Task<List<Gym>> GetAllGymsAsync()
        {
            return await _repository.GetAllAsync();
        }

        public async Task<Gym> GetGymByIdAsync(int id)
        {
            var gym = await _repository.GetByIdAsync(id);

            if (gym == null)
                throw new NotFoundException($"Gym with id = {id} not found");

            return gym;
        }

        public async Task<int> CreateGymAsync(Gym gym)
        {
            if (string.IsNullOrWhiteSpace(gym.Name))
                throw new ValidationException("Gym name cannot be empty");

            return await _repository.AddAsync(gym);
        }

        public async Task UpdateGymAsync(int id, UpdateGymDto dto)
        {
            var gym = await _repository.GetByIdAsync(id);

            if (gym == null)
                throw new NotFoundException($"Gym with id = {id} not found");

            gym.Name = dto.Name;
            gym.Address = dto.Address;
            gym.PhoneNumber = dto.PhoneNumber;

            await _repository.UpdateAsync(gym);
        }

        public async Task DeleteGymAsync(int id)
        {
            var gym = await _repository.GetByIdAsync(id);

            if (gym == null)
                throw new NotFoundException($"Gym with id = {id} not found");

            var hasMembers = await _repository.HasMembersAsync(id);

            if (hasMembers)
                throw new BusinessRuleException($"Gym with id = {id} has members and cannot be deleted");

            gym.IsDeleted = true;
            await _repository.UpdateAsync(gym);
        }
    }
}
