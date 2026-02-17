using GymManagementSystem.Application.DTOs;
using GymManagementSystem.Application.Exceptions;
using GymManagementSystem.Application.Interfaces;
using GymManagementSystem.Domain.Entities;

namespace GymManagementSystem.Application.Services
{
    public class GymService : IGymService
    {
        private readonly IUnitOfWork _unitOfWork;

        public GymService(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        public async Task<List<Gym>> GetAllGymsAsync()
        {
            return await _unitOfWork.Gyms.GetAllAsync();
        }

        public async Task<Gym> GetGymByIdAsync(int id)
        {
            var gym = await _unitOfWork.Gyms.GetByIdAsync(id);

            if (gym == null)
                throw new NotFoundException($"Gym with id = {id} not found");

            return gym;
        }

        public async Task<int> CreateGymAsync(Gym gym)
        {
            if (string.IsNullOrWhiteSpace(gym.Name))
                throw new ValidationException("Gym name cannot be empty");

            var result = await _unitOfWork.Gyms.AddAsync(gym);
            await _unitOfWork.SaveChangesAsync();
            return result;
        }

        public async Task UpdateGymAsync(int id, UpdateGymDto dto)
        {
            var gym = await _unitOfWork.Gyms.GetByIdAsync(id);

            if (gym == null)
                throw new NotFoundException($"Gym with id = {id} not found");

            gym.Name = dto.Name;
            gym.Address = dto.Address;
            gym.PhoneNumber = dto.PhoneNumber;

            await _unitOfWork.Gyms.UpdateAsync(gym);
            await _unitOfWork.SaveChangesAsync();
        }

        public async Task DeleteGymAsync(int id)
        {
            var gym = await _unitOfWork.Gyms.GetByIdAsync(id);

            if (gym == null)
                throw new NotFoundException($"Gym with id = {id} not found");

            var hasMembers = await _unitOfWork.Gyms.HasMembersAsync(id);

            if (hasMembers)
                throw new BusinessRuleException($"Gym with id = {id} has members and cannot be deleted");

            gym.IsDeleted = true;
            await _unitOfWork.Gyms.UpdateAsync(gym);
            await _unitOfWork.SaveChangesAsync();
        }
    }
}