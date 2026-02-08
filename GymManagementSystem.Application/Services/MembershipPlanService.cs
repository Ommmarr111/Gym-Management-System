using GymManagementSystem.Application.DTOs;
using GymManagementSystem.Application.Interfaces;
using GymManagementSystem.Domain.Entities;

namespace GymManagementSystem.Application.Services
{
    public class MembershipPlanService : IMembershipPlanService
    {
        private readonly IMembershipPlanRepository _repository;

        public MembershipPlanService(IMembershipPlanRepository repository)
        {
            _repository = repository;
        }

        public async Task<List<MembershipPlanDto>> GetAllPlansAsync()
        {
            var plans = await _repository.GetAllAsync();

            return plans.Select(p => new MembershipPlanDto
            {
                Id = p.Id,
                Name = p.Name,
                Price = p.Price,
                DurationInDays = p.DurationInDays,
                Description = p.Description,
                GymId = p.GymId,
                GymName = p.Gym != null ? p.Gym.Name : "No Gym Assigned"
            }).ToList();
        }

        public async Task<MembershipPlanDto?> GetPlanByIdAsync(int id)
        {
            var plan = await _repository.GetByIdAsync(id);
            if (plan == null) return null;

            return new MembershipPlanDto
            {
                Id = plan.Id,
                Name = plan.Name,
                Price = plan.Price,
                DurationInDays = plan.DurationInDays,
                Description = plan.Description,
                GymId = plan.GymId,
                GymName = plan.Gym != null ? plan.Gym.Name : "No Gym Assigned"
            };
        }

        public async Task<MembershipPlanDto> CreatePlanAsync(CreateMembershipPlanDto planDto)
        {
            if (planDto.Price <= 0)
                throw new Exception("Price must be greater than zero!");

            var newPlan = new MembershipPlan
            {
                Name = planDto.Name,
                Price = planDto.Price,
                DurationInDays = planDto.DurationInDays,
                Description = planDto.Description,
                GymId = planDto.GymId
            };
            var createdPlan = await _repository.AddAsync(newPlan);
            return new MembershipPlanDto
            {
                Id = createdPlan.Id,
                Name = createdPlan.Name,
                Price = createdPlan.Price,
                DurationInDays = createdPlan.DurationInDays,
                Description = createdPlan.Description,
                GymId = createdPlan.GymId
            };
        }
        public async Task<bool> UpdatePlanAsync(int id, CreateMembershipPlanDto planDto)
        {
            var existingPlan = await _repository.GetByIdAsync(id);

            if (existingPlan == null) return false;
            existingPlan.Name = planDto.Name;
            existingPlan.Price = planDto.Price;
            existingPlan.DurationInDays = planDto.DurationInDays;
            existingPlan.Description = planDto.Description;
            existingPlan.GymId = planDto.GymId;

            await _repository.UpdateAsync(existingPlan);
            return true;
        }

        public async Task<bool> DeletePlanAsync(int id)
        {
            var existingPlan = await _repository.GetByIdAsync(id);
            if (existingPlan == null) return false;
            await _repository.DeleteAsync(id);
            return true;
        }
    }
}