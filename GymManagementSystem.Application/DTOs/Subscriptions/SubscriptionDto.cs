namespace GymManagementSystem.Application.DTOs
{
    public class SubscriptionDto
    {
        public int Id { get; set; }
        public int MemberId { get; set; }
        public string MemberName { get; set; } = string.Empty;
        public string PlanName { get; set; } = string.Empty;
        public decimal Price { get; set; }
        public string Status { get; set; } = string.Empty;
        public string EndDate { get; set; } = string.Empty;
    }
}