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
            return await _context.MembershipPlans.FindAsync(id);
        }

        public async Task<int> AddAsync(MembershipPlan plan)
        {
            await _context.MembershipPlans.AddAsync(plan);
            await _context.SaveChangesAsync();
            return plan.Id;
        }

        public async Task UpdateAsync(MembershipPlan plan)
        {
            _context.MembershipPlans.Update(plan);
            await _context.SaveChangesAsync();
        }

        public async Task DeleteAsync(int id)
        {
            var plan = await _context.MembershipPlans.FindAsync(id);
            if (plan != null)
            {
                // Soft Delete
                plan.IsDeleted = true;
                await _context.SaveChangesAsync();
            }
        }
    }
}
