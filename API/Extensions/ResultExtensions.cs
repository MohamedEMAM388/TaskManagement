using Application.Common.ResultPattern;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace API.Extensions;

public static class ResultExtensions
{
    public static ObjectResult ToProblem(this Result result)
    {
        if (result.IsSuccess)
            throw new InvalidOperationException("A successful result cannot be converted to a problem response.");

        var firstError = result.Errors.FirstOrDefault() ?? Error.Failure();
        var (statusCode, title) = Map(firstError.ErrorType);

        var payload = new
        {
            status = statusCode,
            title,
            errors = result.Errors.Select(e => new { code = e.Code, description = e.Description })
        };

        return new ObjectResult(payload) { StatusCode = statusCode };
    }

    private static (int StatusCode, string Title) Map(ErrorType errorType) => errorType switch
    {
        ErrorType.Validation         => (StatusCodes.Status400BadRequest, "Validation failed."),
        ErrorType.NotFound           => (StatusCodes.Status404NotFound, "Resource not found."),
        ErrorType.Conflict           => (StatusCodes.Status409Conflict, "Conflict."),
        ErrorType.Unauthorized       => (StatusCodes.Status401Unauthorized, "Unauthorized."),
        ErrorType.InvalidCredentials => (StatusCodes.Status401Unauthorized, "Invalid credentials."),
        ErrorType.Forbidden          => (StatusCodes.Status403Forbidden, "Forbidden."),
        _                            => (StatusCodes.Status500InternalServerError, "An unexpected error occurred.")
    };
}
