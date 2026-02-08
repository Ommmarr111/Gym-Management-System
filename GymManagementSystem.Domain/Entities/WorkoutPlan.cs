namespace GymManagementSystem.Domain.Entities
{
    public class WorkoutPlan : BaseEntity
    {
        public string Name { get; set; } = string.Empty;
        public ICollection<Exercise> Exercises { get; set; } = new List<Exercise>();

        public string UserId { get; set; } = string.Empty;
        public ApplicationUser User { get; set; } = null!;
    }
}
