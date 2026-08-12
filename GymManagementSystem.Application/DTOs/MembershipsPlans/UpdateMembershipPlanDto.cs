using System.ComponentModel.DataAnnotations;

namespace GymManagementSystem.Application.DTOs
{
    public class UpdateMembershipPlanDto
    {
        [Required]
        public string Name { get; set; } = string.Empty;

        [Range(0, double.MaxValue)]
        public decimal Price { get; set; }

        [Range(1, 3650)]
        public int DurationInDays { get; set; }
    }
}