namespace Application.Common.Identity;

public class ValidateRefreshTokenResult
{
    public bool IsValid { get; set; }
    public string? UserId { get; init; }

    public string? ErrorMessage { get; set; }
}