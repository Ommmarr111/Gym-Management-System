namespace GymManagementSystem.Domain.Entities
{
    public class Subscription : BaseEntity
    {

        public string UserId { get; set; } = string.Empty;
        public int MembershipPlanId { get; set; }
        public decimal AmountPaid { get; set; }

        public DateTime StartDate { get; set; }
        public DateTime EndDate { get; set; }

        // "Active", "Expired", or "Frozen"
        public string Status { get; set; } = "Active";

        public ApplicationUser User { get; set; } = null!;
        public MembershipPlan MembershipPlan { get; set; } = null!;
    }
}
