
namespace GymManagementSystem.Application.DTOs
{
    public class MemberDetailsDto
    {
        public int Id { get; set; }
        public string FirstName { get; set; } = string.Empty;
        public string LastName { get; set; } = string.Empty;
        public string Email { get; set; } = string.Empty;
        public string PhoneNumber { get; set; } = string.Empty;
        public DateTime DateOfBirth { get; set; }
        public DateTime JoinDate { get; set; }
        public int GymId { get; set; }
        public string GymName { get; set; } = string.Empty;
    }
}
