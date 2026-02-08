namespace GymManagementSystem.Application.DTOs
{
    public class SubscriptionDto
    {
        public int Id { get; set; }

        public int MemberId { get; set; }
        public string MemberName { get; set; } = string.Empty;

        public int MembershipPlanId { get; set; }
        public string PlanName { get; set; } = string.Empty;

        public decimal PricePaid { get; set; }
        public DateTime StartDate { get; set; }
        public DateTime EndDate { get; set; }
        public string Status { get; set; } = string.Empty;
    }
}