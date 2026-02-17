using GymManagementSystem.Application.Interfaces;
using GymManagementSystem.Infrastructure.Persistence;

namespace GymManagementSystem.Infrastructure.Repositories
{
    public class UnitOfWork : IUnitOfWork
    {
        private readonly ApplicationDbContext _context;

        private IGymRepository? _gyms;
        private IMemberRepository? _members;
        private IMembershipPlanRepository? _membershipPlans;
        private ISubscriptionRepository? _subscriptions;
        private IAttendanceRepository? _attendances;

        public UnitOfWork(ApplicationDbContext context)
        {
            _context = context;
        }

        public IGymRepository Gyms =>
            _gyms ??= new GymRepository(_context);

        public IMemberRepository Members =>
            _members ??= new MemberRepository(_context);

        public IMembershipPlanRepository MembershipPlans =>
            _membershipPlans ??= new MembershipPlanRepository(_context);

        public ISubscriptionRepository Subscriptions =>
            _subscriptions ??= new SubscriptionRepository(_context);

        public IAttendanceRepository Attendances =>
            _attendances ??= new AttendanceRepository(_context);

        public async Task<int> SaveChangesAsync(CancellationToken cancellationToken = default)
        {
            return await _context.SaveChangesAsync(cancellationToken);
        }

        public void Dispose()
        {
            _context.Dispose();
        }
    }
}