using AutoMapper;
using GymManagementSystem.Application.DTOs;
using GymManagementSystem.Application.Exceptions;
using GymManagementSystem.Application.Interfaces;
using GymManagementSystem.Domain.Entities;
using Microsoft.Extensions.Caching.Memory;

namespace GymManagementSystem.Application.Services
{
    public class MembershipPlanService : IMembershipPlanService
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IMapper _mapper;
        private readonly IMemoryCache _cache;

        public MembershipPlanService(IUnitOfWork unitOfWork, IMapper mapper, IMemoryCache memoryCache)
        {
            _unitOfWork = unitOfWork;
            _mapper = mapper;
            _cache = memoryCache;
        }

        public async Task<List<MembershipPlanDto>> GetAllPlansAsync()
        {
            const string cacheKey = "plans";

            // 1. Check cache
            var cachedPlans = _cache.Get<List<MembershipPlanDto>>(cacheKey);

            if (cachedPlans is not null)

                return cachedPlans;


            // 2. Cache miss → Database
            var plans = await _unitOfWork.MembershipPlans.GetAllAsync();

            var planDtos = _mapper.Map<List<MembershipPlanDto>>(plans);

            // 3. Store in cache
            _cache.Set(
                cacheKey,
                planDtos,
                TimeSpan.FromMinutes(10));
            return planDtos;
        }

        public async Task<MembershipPlanDto> GetPlanByIdAsync(int id)
        {
            var cacheKey = $"plan:{id}";

            // 1. Cache
            var cachedPlan = _cache.Get<MembershipPlanDto>(cacheKey);

            if (cachedPlan is not null)

                return cachedPlan;

            // 2. Cache miss → DB
            var plan = await _unitOfWork.MembershipPlans.GetByIdAsync(id);

            if (plan == null)
                throw new NotFoundException(
                    $"Membership plan with id = {id} not found");

            // 3. Map
            var planDto = _mapper.Map<MembershipPlanDto>(plan);

            // 4. Cache
            _cache.Set(
                cacheKey,
                planDto,
                TimeSpan.FromMinutes(10));

            // 5. Return
            return planDto;
        }

        public async Task<MembershipPlanDto> CreatePlanAsync(CreateMembershipPlanDto planDto)
        {
            var newPlan = _mapper.Map<MembershipPlan>(planDto);

            var createdPlan = await _unitOfWork.MembershipPlans.AddAsync(newPlan);
            await _unitOfWork.SaveChangesAsync();


            _cache.Remove("plans");
            _cache.Remove($"plans:gym:{createdPlan.GymId}");

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
            _cache.Remove("plans");
            _cache.Remove($"plan:{id}");
            _cache.Remove($"plans:gym:{plan.GymId}");
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

            _cache.Remove("plans");
            _cache.Remove($"plan:{id}");
            _cache.Remove($"plans:gym:{plan.GymId}");
        }

        public async Task<List<MembershipPlanDto>> GetPlansByGymIdAsync(int gymId)
        {
            var gym = await _unitOfWork.Gyms.GetByIdAsync(gymId);

            if (gym == null)
                throw new NotFoundException($"Gym with id = {gymId} not found");

            var cacheKey = $"plans:gym:{gymId}";

            // 1. Check cache

            var cachedPlans = _cache.Get<List<MembershipPlanDto>>(cacheKey);

            if (cachedPlans is not null)
            {
                return cachedPlans;
            }

            var plans = await _unitOfWork.MembershipPlans.GetByGymIdAsync(gymId);

            var planDtos = _mapper.Map<List<MembershipPlanDto>>(plans);

            // 2. Store in cache

            _cache.Set(
                cacheKey,
                planDtos,
                TimeSpan.FromMinutes(10));

            return planDtos;
        }
    }
}