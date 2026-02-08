using System.ComponentModel.DataAnnotations;

namespace GymManagementSystem.Api.DTOs
{
    public class CreateMembershipPlanDto
    {
        [Required(ErrorMessage = "Name is required !")]
        public string Name { get; set; } = string.Empty;
        [Range(1, 10000)]
        public decimal Price { get; set; }
        [Required]
        public string Description { get; set; } = string.Empty;
        [Range(1, 365)]
        public int DurationInDays { get; set; }
        [Required]
        public int GymId { get; set; }
    }
}