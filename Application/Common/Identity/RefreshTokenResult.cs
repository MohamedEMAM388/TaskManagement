namespace Application.Common.Identity;

public class RefreshTokenResult
{
    public string Token { get; init; } = string.Empty;
    public DateTime ExpiresOn { get; init; }
    public DateTime CreatedOn { get; init; } = DateTime.UtcNow;
}