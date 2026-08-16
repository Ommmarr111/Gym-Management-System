using Microsoft.EntityFrameworkCore.Storage;
using System.Data;

namespace GymManagementSystem.Application.Interfaces
{
    public interface IUnitOfWork : IDisposable
    {
        IGymRepository Gyms { get; }
        IMemberRepository Members { get; }
        IMembershipPlanRepository MembershipPlans { get; }
        ISubscriptionRepository Subscriptions { get; }
        IAttendanceRepository Attendances { get; }
        IPaymentRepository Payments { get; }

        IRefreshTokenRepository RefreshTokens { get; }

        Task<IDbContextTransaction> BeginTransactionAsync();

        Task<IDbContextTransaction> BeginTransactionAsync(IsolationLevel isolationLevel);
        Task<int> SaveChangesAsync(CancellationToken cancellationToken = default);
    }
}