namespace GymManagementSystem.Domain.Entities
{
    public class Attendance : BaseEntity
    {
        public int MemberId { get; set; }
        public Member Member { get; set; } = null!;
        public int GymId { get; set; }
        public Gym Gym { get; set; } = null!;
        public DateTime CheckInTime { get; set; } = DateTime.UtcNow;
        public DateTime? CheckOutTime { get; set; }
    }
}