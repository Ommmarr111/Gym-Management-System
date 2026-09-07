namespace GymManagementSystem.Application.DTOs.Members
{
    public class MemberDetailsWithSubscriptionDto : MemberDetailsDto
    {
        public string? ActiveSubscriptionStatus { get; set; }
        public DateTime? ActiveSubscriptionEndDate { get; set; }
    }
}