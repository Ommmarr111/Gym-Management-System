namespace GymManagementSystem.Domain.Entities
{
    public class Subscription : BaseEntity
    {
        public int MemberId { get; set; }
        public int MembershipPlanId { get; set; }
        public DateTime StartDate { get; set; }
        public DateTime EndDate { get; set; }
        public decimal AmountPaid { get; set; }

        public string Status { get; set; } = "Active";     // Values: "Pending", "Active", "Frozen", "Expired", "Cancelled"


        public DateTime? FrozenDate { get; set; }
        public int? FrozenDurationDays { get; set; }

        public Member Member { get; set; } = null!;
        public MembershipPlan MembershipPlan { get; set; } = null!;
    }
}