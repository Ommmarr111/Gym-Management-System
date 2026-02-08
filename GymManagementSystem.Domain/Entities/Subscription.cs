namespace GymManagementSystem.Domain.Entities
{
    public class Subscription : BaseEntity
    {
        public int MemberId { get; set; }
        public Member Member { get; set; } = null!;
        public int MembershipPlanId { get; set; }
        public MembershipPlan MembershipPlan { get; set; } = null!;
        public decimal AmountPaid { get; set; }
        public DateTime StartDate { get; set; }
        public DateTime EndDate { get; set; }
        public string Status { get; set; } = "Active";
    }
}