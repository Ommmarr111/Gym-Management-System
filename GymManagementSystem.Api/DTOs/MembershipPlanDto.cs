namespace GymManagementSystem.Api.DTOs
{
    public class MembershipPlanDto
    {
        public int Id { get; set; }
        public string Name { get; set; } = string.Empty;
        public decimal Price { get; set; }
        public string Description { get; set; } = string.Empty;
        public int DurationInDays { get; set; }
        public int GymId { get; set; }
        public string GymName { get; set; } = string.Empty;
    }
}