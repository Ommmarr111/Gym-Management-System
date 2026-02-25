using Microsoft.AspNetCore.Identity;

namespace GymManagementSystem.Domain.Entities
{
    public class ApplicationUser : IdentityUser
    {
        public string FullName { get; set; } = string.Empty;
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
        public bool IsDeleted { get; set; } = false;

        public ICollection<Subscription> Subscriptions { get; set; } = new List<Subscription>();
    }
}
