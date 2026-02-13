using System.ComponentModel.DataAnnotations;

namespace GymManagementSystem.Application.DTOs
{
    public class UpdateGymDto
    {
        [Required]
        public string Name { get; set; } = string.Empty;

        [Required]
        public string Address { get; set; } = string.Empty;

        [Phone]
        public string PhoneNumber { get; set; } = string.Empty;
    }
}