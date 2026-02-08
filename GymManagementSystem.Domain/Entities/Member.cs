namespace GymManagementSystem.Domain.Entities
{
    public class Member : BaseEntity
    {
        public string FirstName { get; set; } = string.Empty;
        public string LastName { get; set; } = string.Empty;

        public string Email { get; set; } = string.Empty;
        public string PhoneNumber { get; set; } = string.Empty;

        public DateTime DateOfBirth { get; set; }
        public DateTime JoinDate { get; set; } = DateTime.UtcNow;

        public int GymId { get; set; }
        public Gym Gym { get; set; } = null!;
    }
}