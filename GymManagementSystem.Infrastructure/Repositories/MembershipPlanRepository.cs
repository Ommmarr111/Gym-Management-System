using GymManagementSystem.Application.Interfaces;
using GymManagementSystem.Domain.Entities;
using GymManagementSystem.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;

namespace GymManagementSystem.Infrastructure.Repositories
{
    public class MembershipPlanRepository : IMembershipPlanRepository
    {
        private readonly ApplicationDbContext _context;

        public MembershipPlanRepository(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<List<MembershipPlan>> GetAllAsync()
        {
            return await _context.MembershipPlans
                .Include(p => p.Gym)
                .ToListAsync();
        }

        public async Task<MembershipPlan?> GetByIdAsync(int id)
        {
            return await _context.MembershipPlans
                .Include(p => p.Gym)
                .FirstOrDefaultAsync(p => p.Id == id);
        }
        public async Task<MembershipPlan> AddAsync(MembershipPlan plan)
        {
            await _context.MembershipPlans.AddAsync(plan);
            return plan;
        }

        public Task UpdateAsync(MembershipPlan plan)
        {
            _context.MembershipPlans.Update(plan);
            return Task.CompletedTask;
        }

        public async Task DeleteAsync(int id)
        {
            var plan = await _context.MembershipPlans.FindAsync(id);
            if (plan != null)
            {
                plan.IsDeleted = true;
            }
        }

        public async Task<bool> HasActiveSubscriptionsAsync(int planId)
        {
            return await _context.Subscriptions
                .AnyAsync(s => s.MembershipPlanId == planId && s.EndDate >= DateTime.Now && !s.IsDeleted);
        }

        public async Task<List<MembershipPlan>> GetByGymIdAsync(int gymId)
        {
            return await _context.MembershipPlans
                .Include(p => p.Gym)
                .Where(p => p.GymId == gymId && !p.IsDeleted)
                .ToListAsync();
        }
    }
}