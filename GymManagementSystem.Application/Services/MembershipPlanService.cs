using AutoMapper;
using GymManagementSystem.Application.Common.Caching;
using GymManagementSystem.Application.DTOs;
using GymManagementSystem.Application.Exceptions;
using GymManagementSystem.Application.Interfaces;
using GymManagementSystem.Domain.Entities;
using Microsoft.Extensions.Caching.Distributed;
using Microsoft.Extensions.Caching.Memory;
using System.Text.Json;

namespace GymManagementSystem.Application.Services
{
    public class MembershipPlanService : IMembershipPlanService
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IMapper _mapper;
        private readonly IMemoryCache _cache;
        private readonly IDistributedCache _redisCache;

        public MembershipPlanService(IUnitOfWork unitOfWork, IMapper mapper, IMemoryCache memoryCache, IDistributedCache cache)
        {
            _unitOfWork = unitOfWork;
            _mapper = mapper;
            _cache = memoryCache;
            _redisCache = cache;
        }

        public async Task<List<MembershipPlanDto>> GetAllPlansAsync()
        {

            return await _cache.GetOrCreateAsync(
                CacheKeys.Plans.All,
                async entry =>
                {
                    entry.AbsoluteExpirationRelativeToNow = TimeSpan.FromMinutes(10);
                    var plans = await _unitOfWork.MembershipPlans.GetAllAsync();
                    return _mapper.Map<List<MembershipPlanDto>>(plans);
                }) ?? new List<MembershipPlanDto>();
        }

        public async Task<MembershipPlanDto> GetPlanByIdAsync(int id)
        {
            var result = await _cache.GetOrCreateAsync(
                CacheKeys.Plans.ById(id),
                async entry =>
                {
                    entry.AbsoluteExpirationRelativeToNow = TimeSpan.FromMinutes(10);
                    var plan = await _unitOfWork.MembershipPlans.GetByIdAsync(id);
                    if (plan == null)
                        throw new NotFoundException($"Membership plan with id = {id} not found");
                    return _mapper.Map<MembershipPlanDto>(plan);
                });

            return result!; // safe: factory guarantees non-null or throws before returning
        }

        public async Task<MembershipPlanDto> CreatePlanAsync(CreateMembershipPlanDto planDto)
        {
            var newPlan = _mapper.Map<MembershipPlan>(planDto);

            var createdPlan = await _unitOfWork.MembershipPlans.AddAsync(newPlan);
            await _unitOfWork.SaveChangesAsync();


            await InvalidatePlanCachesAsync(createdPlan);

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
            await InvalidatePlanCachesAsync(plan);
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

            await InvalidatePlanCachesAsync(plan);
        }

        public async Task<List<MembershipPlanDto>> GetPlansByGymIdAsync(int gymId)
        {
            var gym = await _unitOfWork.Gyms.GetByIdAsync(gymId);
            if (gym == null)
                throw new NotFoundException($"Gym with id = {gymId} not found");

            var cacheKey = CacheKeys.Plans.ByGymId(gymId);

            return await GetOrSetAsync(cacheKey, async () =>
            {
                var plans = await _unitOfWork.MembershipPlans.GetByGymIdAsync(gymId);
                return _mapper.Map<List<MembershipPlanDto>>(plans) ?? new List<MembershipPlanDto>();
            });
        }
        private async Task InvalidatePlanCachesAsync(MembershipPlan plan)
        {
            _cache.Remove(CacheKeys.Plans.All);
            _cache.Remove(CacheKeys.Plans.ById(plan.Id));
            await _redisCache.RemoveAsync(CacheKeys.Plans.ByGymId(plan.GymId));
        }

        private async Task<T> GetOrSetAsync<T>(
    string cacheKey,                  // the Redis key, e.g. "plans:gym:3"
    Func<Task<T>> factory,            // the "how to get it if it's missing" function
    TimeSpan? expiration = null)      // how long to keep it cached (optional, defaults below)
        {
            // 1. Try Redis first
            var cachedData = await _redisCache.GetStringAsync(cacheKey);

            if (cachedData is not null)
            {
                var cachedValue = JsonSerializer.Deserialize<T>(cachedData);
                if (cachedValue is not null)
                    return cachedValue;
            }

            // 2. Cache miss → call the factory to get the real value
            var value = await factory();

            // 3. Store it in Redis for next time
            var serialized = JsonSerializer.Serialize(value);
            await _redisCache.SetStringAsync(
                cacheKey,
                serialized,
                new DistributedCacheEntryOptions
                {
                    AbsoluteExpirationRelativeToNow = expiration ?? TimeSpan.FromMinutes(10)
                });

            return value;
        }
    }
}