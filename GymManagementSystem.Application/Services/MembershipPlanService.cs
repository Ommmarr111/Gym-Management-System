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
        public async Task<List<MembershipPlan>> GetAllPlansAsync()
        {
            return await _repository.GetAllAsync();
        }

        public async Task<MembershipPlan?> GetPlanByIdAsync(int id)
        {
            return await _repository.GetByIdAsync(id);
        }

        public async Task<int> CreatePlanAsync(MembershipPlan plan)
        {
            if (plan.Price <= 0)
            {
                throw new Exception("Price must be greater than zero!");
            }

            if (string.IsNullOrEmpty(plan.Description))
            {
                throw new Exception("Description is required.");
            }
            return await _repository.AddAsync(plan);
        }

    }
}
