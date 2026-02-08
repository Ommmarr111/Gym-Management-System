using System.ComponentModel.DataAnnotations;

namespace GymManagementSystem.Application.DTOs.Auth
{
    public class RegisterDto
    {
        [Required(ErrorMessage = "Name is required !")]
        public string FullName { get; set; } = string.Empty;

        [Required]
        [EmailAddress]
        public string Email { get; set; } = string.Empty;

        [Required]
        [MinLength(6)]
        public string Password { get; set; } = string.Empty;

        // Bonus: Allow them to pick a role? (For now, let's default to "Trainee")
    }
}