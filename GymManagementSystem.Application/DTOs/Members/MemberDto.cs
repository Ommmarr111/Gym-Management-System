namespace GymManagementSystem.Application.DTOs
{
    public class MemberDto
    {
        public int Id { get; set; }
        public string FullName { get; set; } = string.Empty;
        public string Email { get; set; } = string.Empty;
        public string GymName { get; set; } = string.Empty;
    }
}