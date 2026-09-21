namespace Application.Common.ResultPattern;

public enum ErrorType
{
    Failure,
    Validation,
    NotFound,
    Conflict,
    Unauthorized,
    Forbidden,
    InvalidCredentials,
}