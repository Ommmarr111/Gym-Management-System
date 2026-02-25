using AutoMapper;
using GymManagementSystem.Application.DTOs;
using GymManagementSystem.Application.Exceptions;
using GymManagementSystem.Application.Interfaces;
using GymManagementSystem.Domain.Entities;

namespace GymManagementSystem.Application.Services
{
    public class GymService : IGymService
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IMapper _mapper;

        public GymService(IUnitOfWork unitOfWork, IMapper mapper)
        {
            _unitOfWork = unitOfWork;
            _mapper = mapper;
        }

        public async Task<List<GymDto>> GetAllGymsAsync()
        {
            var gyms = await _unitOfWork.Gyms.GetAllAsync();
            return _mapper.Map<List<GymDto>>(gyms);
        }

        public async Task<GymDto> GetGymByIdAsync(int id)
        {
            var gym = await _unitOfWork.Gyms.GetByIdAsync(id);

            if (gym == null)
                throw new NotFoundException($"Gym with id = {id} not found");

            return _mapper.Map<GymDto>(gym);
        }

        public async Task<GymDto> CreateGymAsync(CreateGymDto dto)
        {
            var gym = _mapper.Map<Gym>(dto);

            await _unitOfWork.Gyms.AddAsync(gym);
            await _unitOfWork.SaveChangesAsync();

            return _mapper.Map<GymDto>(gym);
        }

        public async Task UpdateGymAsync(int id, UpdateGymDto dto)
        {
            var gym = await _unitOfWork.Gyms.GetByIdAsync(id);

            if (gym == null)
                throw new NotFoundException($"Gym with id = {id} not found");

            // ✅ ADD THIS BLOCK
            if (dto.Capacity < gym.Capacity)
            {
                var memberCount = await _unitOfWork.Members.CountByGymIdAsync(id);
                if (dto.Capacity < memberCount)
                    throw new BusinessRuleException(
                        $"Cannot reduce capacity to {dto.Capacity}. Gym currently has {memberCount} members.");
            }

            _mapper.Map(dto, gym);
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