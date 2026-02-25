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

        Task<int> SaveChangesAsync(CancellationToken cancellationToken = default);
    }
}