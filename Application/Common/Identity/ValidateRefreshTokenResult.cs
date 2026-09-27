namespace Application.Common.Identity;

public class ValidateRefreshTokenResult
{
    public bool IsValid { get; init; }
    public string? UserId { get; init; }

    public string? ErrorMessage { get; init; }
}