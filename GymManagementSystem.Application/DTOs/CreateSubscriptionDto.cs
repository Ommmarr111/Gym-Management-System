using System.ComponentModel.DataAnnotations;

namespace GymManagementSystem.Application.DTOs
{
    public class CreateSubscriptionDto
    {
        [Required]
        public int MemberId { get; set; }

        [Required]
        public int MembershipPlanId { get; set; }
    }
}