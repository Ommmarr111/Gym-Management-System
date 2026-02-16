using GymManagementSystem.Application.DTOs;
using GymManagementSystem.Application.Exceptions;
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

            if (plan == null)
                throw new NotFoundException($"Membership plan with id = {id} not found");

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
                throw new ValidationException("Price must be greater than zero!");

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

        public async Task UpdatePlanAsync(int id, UpdateMembershipPlanDto dto)
        {
            var plan = await _repository.GetByIdAsync(id);

            if (plan == null)
                throw new NotFoundException($"Membership plan with id = {id} not found");

            plan.Name = dto.Name;
            plan.Price = dto.Price;
            plan.DurationInDays = dto.DurationInDays;

            await _repository.UpdateAsync(plan);
        }

        public async Task DeletePlanAsync(int id)
        {
            var plan = await _repository.GetByIdAsync(id);

            if (plan == null)
                throw new NotFoundException($"Membership plan with id = {id} not found");

            var hasActiveSubs = await _repository.HasActiveSubscriptionsAsync(id);

            if (hasActiveSubs)
                throw new BusinessRuleException($"Membership plan with id = {id} has active subscriptions and cannot be deleted");

            plan.IsDeleted = true;
            await _repository.UpdateAsync(plan);
        }
    }
}
