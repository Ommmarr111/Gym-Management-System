using GymManagementSystem.Application.Interfaces;
using GymManagementSystem.Domain.Entities;
using GymManagementSystem.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;

namespace GymManagementSystem.Infrastructure.Repositories
{
    public class SubscriptionRepository : ISubscriptionRepository
    {
        private readonly ApplicationDbContext _context;

        public SubscriptionRepository(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<List<Subscription>> GetAllAsync()
        {
            return await _context.Subscriptions
                .Include(s => s.Member)
                .Include(s => s.MembershipPlan)
                .OrderByDescending(s => s.StartDate)
                .ToListAsync();
        }

        public async Task<Subscription?> GetByIdAsync(int id)
        {
            return await _context.Subscriptions
                .Include(s => s.Member)
                .Include(s => s.MembershipPlan)
                .FirstOrDefaultAsync(s => s.Id == id);
        }

        public async Task<Subscription> AddAsync(Subscription subscription)
        {
            await _context.Subscriptions.AddAsync(subscription);
            return subscription;
        }

        public async Task<List<Subscription>> GetByMemberIdAsync(int memberId)
        {
            return await _context.Subscriptions
                .Where(s => s.MemberId == memberId)
                .Include(s => s.MembershipPlan)
                .Include(s => s.Member)
                .OrderByDescending(s => s.StartDate)
                .ToListAsync();
        }
        public async Task UpdateStatusAsync(int subscriptionId, string newStatus)
        {
            var sub = await _context.Subscriptions.FindAsync(subscriptionId);
            if (sub != null)
            {
                sub.Status = newStatus;
            }
        }
        public async Task<Subscription?> GetActiveSubscriptionAsync(int memberId, int membershipPlanId)
        {
            return await _context.Subscriptions
                .FirstOrDefaultAsync(s =>
                    s.MemberId == memberId &&
                    s.MembershipPlanId == membershipPlanId &&
                    s.Status == "Active" &&
                    s.EndDate.Date >= DateTime.UtcNow.Date);
        }
    }
}