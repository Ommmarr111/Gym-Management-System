using GymManagementSystem.Application.DTOs.Shared;

namespace GymManagementSystem.Application.DTOs.Subscriptions
{
    public class SubscriptionRequestParams : PaginationParams
    {
        public int? MemberId { get; set; }
        public int? MembershipPlanId { get; set; }
        public string? Status { get; set; } // Values: "Pending", "Active", "Frozen", "Expired", "Cancelled"


        // Date filters are crucial for financial/subscription records
        public DateTime? StartDate { get; set; }
        public DateTime? EndDate { get; set; }
    }
}
