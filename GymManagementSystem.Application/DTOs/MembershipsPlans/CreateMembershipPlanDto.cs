namespace GymManagementSystem.Application.DTOs
{
    public class CreateMembershipPlanDto
    {
        public string Name { get; set; } = string.Empty;
        public decimal Price { get; set; }
        public string Description { get; set; } = string.Empty;
        public int DurationInDays { get; set; }
        public int GymId { get; set; }
    }
}