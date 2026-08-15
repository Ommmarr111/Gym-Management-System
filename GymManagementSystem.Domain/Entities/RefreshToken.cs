using GymManagementSystem.Domain.Entities;

public class RefreshToken
{
    public Guid Id { get; set; } = Guid.NewGuid();

    public string TokenHash { get; set; } = string.Empty;

    public string UserId { get; set; } = string.Empty;

    public ApplicationUser User { get; set; } = null!;

    public DateTime CreatedOn { get; set; } = DateTime.UtcNow;

    public DateTime ExpiresOn { get; set; }

    public DateTime? RevokedOn { get; set; }


    public bool IsExpired => DateTime.UtcNow >= ExpiresOn;

    public bool IsRevoked => RevokedOn.HasValue;

    public bool IsActive => !IsExpired && !IsRevoked;
}