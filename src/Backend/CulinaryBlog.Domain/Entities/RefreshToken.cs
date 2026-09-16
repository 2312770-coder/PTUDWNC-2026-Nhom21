namespace CulinaryBlog.Domain.Entities;

// SRS mục 7.8 - KHÔNG lưu raw token, chỉ lưu SHA-256 hash của nó.
// ReplacedByTokenHash dùng để trace "token family" khi rotation - nếu phát hiện
// token đã bị thay thế mà vẫn được dùng lại thì nghi ngờ bị đánh cắp (FR-AUTH-004).
public class RefreshToken
{
    public Guid Id { get; private set; }
    public string UserId { get; private set; } = string.Empty;
    public string TokenHash { get; private set; } = string.Empty;
    public DateTime ExpiresAt { get; private set; }
    public DateTime? RevokedAt { get; private set; }
    public string? ReplacedByTokenHash { get; private set; }
    public DateTime CreatedAt { get; private set; }
    public string? CreatedByIp { get; private set; }

    public ApplicationUser? User { get; private set; }

    protected RefreshToken() { }

    public static RefreshToken Create(string userId, string tokenHash, int expiryDays, string? createdByIp = null)
        => new()
        {
            Id = Guid.NewGuid(),
            UserId = userId,
            TokenHash = tokenHash,
            ExpiresAt = DateTime.UtcNow.AddDays(expiryDays),
            CreatedAt = DateTime.UtcNow,
            CreatedByIp = createdByIp,
        };

    public bool IsExpired => DateTime.UtcNow >= ExpiresAt;
    public bool IsRevoked => RevokedAt.HasValue;
    public bool IsActive => !IsRevoked && !IsExpired;

    // Thu hồi token. Truyền replacedByTokenHash khi đây là bước rotation.
    public void Revoke(string? replacedByTokenHash = null)
    {
        RevokedAt = DateTime.UtcNow;
        ReplacedByTokenHash = replacedByTokenHash;
    }
}
