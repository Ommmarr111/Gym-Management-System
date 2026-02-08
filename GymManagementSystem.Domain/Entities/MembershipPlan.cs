namespace GymManagementSystem.Domain.Entities
{
    public class MembershipPlan : BaseEntity
    {
        public string Name { get; set; } = string.Empty;
        public int DurationInDays { get; set; }
        public decimal Price { get; set; }
        public string Description { get; set; } = string.Empty;
        public int GymId { get; set; }
        public Gym Gym { get; set; } = null!;
    }
}
