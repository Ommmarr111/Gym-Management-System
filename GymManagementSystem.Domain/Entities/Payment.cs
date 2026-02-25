namespace GymManagementSystem.Domain.Entities
{
    public class Payment : BaseEntity
    {
        public int SubscriptionId { get; set; }
        public decimal Amount { get; set; }
        public DateTime PaymentDate { get; set; } = DateTime.UtcNow;
        public string PaymentMethod { get; set; } = string.Empty;
        public string Status { get; set; } = "Completed";
        public string? TransactionReference { get; set; }
        public Subscription Subscription { get; set; } = null!;
    }
}