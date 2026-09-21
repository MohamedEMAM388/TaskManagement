namespace Application.Common.ResultPattern;

public record Error(string Code, string Description, ErrorType ErrorType = ErrorType.Failure)
{
    public static Error Failure(string code = "General.Failure", string description = "General Failure has Occurred")
        => new(code, description, ErrorType.Failure);

    public static Error Validation(string code = "General.Validation", string description = "General Validation Error has Occurred")
        => new(code, description, ErrorType.Validation);

    public static Error NotFound(string code = "General.NotFound", string description = "General Not Found Error has Occurred")
        => new(code, description, ErrorType.NotFound);

    public static Error Conflict(string code = "General.Conflict", string description = "General Conflict Error has Occurred")
        => new(code, description, ErrorType.Conflict);

    public static Error Unauthorized(string code = "General.Unauthorized", string description = "General Unauthorized Error has Occurred")
        => new(code, description, ErrorType.Unauthorized);

    public static Error Forbidden(string code = "General.Forbidden", string description = "General Forbidden Error has Occurred")
        => new(code, description, ErrorType.Forbidden);

    public static Error InvalidCredentials(string code = "General.InvalidCredentials", string description = "Invalid email or password")
        => new(code, description, ErrorType.InvalidCredentials);
}