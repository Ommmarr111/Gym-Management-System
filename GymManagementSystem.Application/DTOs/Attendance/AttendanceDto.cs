namespace GymManagementSystem.Application.DTOs
{
    public class AttendanceDto
    {
        public int Id { get; set; }
        public string MemberName { get; set; } = string.Empty;
        public string GymName { get; set; } = string.Empty;
        public string CheckInTime { get; set; } = string.Empty;
    }
}