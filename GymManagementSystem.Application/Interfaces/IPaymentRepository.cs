using GymManagementSystem.Domain.Entities;

namespace GymManagementSystem.Application.Interfaces
{
    public interface IPaymentRepository
    {
        Task<List<Payment>> GetAllAsync();
        Task<Payment?> GetByIdAsync(int id);
        Task<Payment> AddAsync(Payment payment);
        Task<List<Payment>> GetBySubscriptionIdAsync(int subscriptionId);
    }
}