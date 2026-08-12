namespace GymManagementSystem.Application.DTOs
{
    public class FreezeSubscriptionDto
    {
        public int DurationDays { get; set; }
        public string Reason { get; set; } = string.Empty;
    }
}