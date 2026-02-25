using AutoMapper;
using GymManagementSystem.Application.DTOs;
using GymManagementSystem.Application.Exceptions;
using GymManagementSystem.Application.Interfaces;
using GymManagementSystem.Domain.Entities;

namespace GymManagementSystem.Application.Services
{
    public class MembershipPlanService : IMembershipPlanService
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IMapper _mapper;

        public MembershipPlanService(IUnitOfWork unitOfWork, IMapper mapper)
        {
            _unitOfWork = unitOfWork;
            _mapper = mapper;
        }

        public async Task<List<MembershipPlanDto>> GetAllPlansAsync()
        {
            var plans = await _unitOfWork.MembershipPlans.GetAllAsync();
            return _mapper.Map<List<MembershipPlanDto>>(plans);
        }

        public async Task<MembershipPlanDto> GetPlanByIdAsync(int id)
        {
            var plan = await _unitOfWork.MembershipPlans.GetByIdAsync(id);

            if (plan == null)
                throw new NotFoundException($"Membership plan with id = {id} not found");

            return _mapper.Map<MembershipPlanDto>(plan);
        }

        public async Task<MembershipPlanDto> CreatePlanAsync(CreateMembershipPlanDto planDto)
        {
            var newPlan = _mapper.Map<MembershipPlan>(planDto);  // ✅ Use mapper

            var createdPlan = await _unitOfWork.MembershipPlans.AddAsync(newPlan);
            await _unitOfWork.SaveChangesAsync();

            return _mapper.Map<MembershipPlanDto>(createdPlan);
        }

        public async Task UpdatePlanAsync(int id, UpdateMembershipPlanDto dto)
        {
            var plan = await _unitOfWork.MembershipPlans.GetByIdAsync(id);

            if (plan == null)
                throw new NotFoundException($"Membership plan with id = {id} not found");

            _mapper.Map(dto, plan);  // ✅ Use mapper to update existing entity

            await _unitOfWork.MembershipPlans.UpdateAsync(plan);
            await _unitOfWork.SaveChangesAsync();
        }

        public async Task DeletePlanAsync(int id)
        {
            var plan = await _unitOfWork.MembershipPlans.GetByIdAsync(id);

            if (plan == null)
                throw new NotFoundException($"Membership plan with id = {id} not found");

            var hasActiveSubs = await _unitOfWork.MembershipPlans.HasActiveSubscriptionsAsync(id);

            if (hasActiveSubs)
                throw new BusinessRuleException($"Membership plan with id = {id} has active subscriptions and cannot be deleted");

            plan.IsDeleted = true;
            await _unitOfWork.MembershipPlans.UpdateAsync(plan);
            await _unitOfWork.SaveChangesAsync();
        }

        public async Task<List<MembershipPlanDto>> GetPlansByGymIdAsync(int gymId)
        {
            var gym = await _unitOfWork.Gyms.GetByIdAsync(gymId);

            if (gym == null)
                throw new NotFoundException($"Gym with id = {gymId} not found");

            var plans = await _unitOfWork.MembershipPlans.GetByGymIdAsync(gymId);

            return _mapper.Map<List<MembershipPlanDto>>(plans);  // ✅ Use mapper
        }
    }
}