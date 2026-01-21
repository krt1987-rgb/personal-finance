using PersonalFinance.Domain.Common;

namespace PersonalFinance.Domain.Entities;

public class PasswordResetToken : BaseEntity
{
    public Guid UserId { get; set; }
    public string Token { get; set; } = string.Empty;
    public DateTime ExpiresAt { get; set; }
    public bool IsUsed { get; set; }
    public DateTime? UsedAt { get; set; }
    
    // Navigation property
    public User User { get; set; } = null!;
    
    public bool IsExpired => DateTime.UtcNow >= ExpiresAt;
    public bool IsValid => !IsUsed && !IsExpired;
}
