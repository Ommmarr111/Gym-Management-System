using System.ComponentModel.DataAnnotations;

namespace GymManagementSystem.Application.DTOs
{
    public class CreateGymDto
    {

        [Required(ErrorMessage = "Gym Name is required!")]
        public string Name { get; set; } = string.Empty;

        [Required]
        public string Address { get; set; } = string.Empty;

        [Phone]
        public string PhoneNumber { get; set; } = string.Empty;

    }
}
