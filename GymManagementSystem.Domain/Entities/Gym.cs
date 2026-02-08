namespace GymManagementSystem.Domain.Entities
{
    public class Gym : BaseEntity
    {
        public string Name { get; set; } = string.Empty;
        public string Address { get; set; } = string.Empty;
        public string PhoneNumber { get; set; } = string.Empty;
        public ICollection<MembershipPlan> Plans { get; set; } = new List<MembershipPlan>();
    }
}
