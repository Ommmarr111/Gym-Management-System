using System.ComponentModel.DataAnnotations;

namespace GymManagementSystem.Application.DTOs
{
    public class CheckInDto
    {
        [Required]
        public int MemberId { get; set; }

        [Required]
        public int GymId { get; set; }
    }
}