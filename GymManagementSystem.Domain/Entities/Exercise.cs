namespace GymManagementSystem.Domain.Entities
{
    public class Exercise : BaseEntity
    {
        public string Name { get; set; } = string.Empty;
        public int Sets { get; set; }
        public int Reps { get; set; }
        public int RestInSeconds { get; set; }

        public int WorkoutPlanId { get; set; }
        public WorkoutPlan WorkoutPlan { get; set; } = null!;
    }
}
