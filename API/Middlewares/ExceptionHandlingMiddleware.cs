using System.Net;
using System.Text.Json;
using Application.Common.Exceptions;
using Domain.Exceptions;
using FluentValidation;

namespace API.Middlewares;

public class ExceptionHandlingMiddleware(RequestDelegate next, ILogger<ExceptionHandlingMiddleware> logger)
{
    public async Task InvokeAsync(HttpContext context)
    {
        try
        {
            await next(context);
        }
        catch (ValidationException ex)
        {
            await WriteResponse(context, HttpStatusCode.BadRequest, "Validation failed.",
                ex.Errors.Select(e => e.ErrorMessage));
        }
        catch (NotFoundException ex)
        {
            await WriteResponse(context, HttpStatusCode.NotFound, ex.Message, []);
        }
        catch (BusinessRuleException ex)
        {
            await WriteResponse(context, HttpStatusCode.BadRequest, ex.Message, []);
        }
        catch (InvalidTaskStatusTransitionException ex)
        {
            await WriteResponse(context, HttpStatusCode.BadRequest, ex.Message, []);
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "Unhandled exception");
            await WriteResponse(context, HttpStatusCode.InternalServerError,
                "An unexpected error occurred.", []);
        }
    }

    private static Task WriteResponse(
        HttpContext context, HttpStatusCode statusCode, string title, IEnumerable<string> errors)
    {
        context.Response.ContentType = "application/json";
        context.Response.StatusCode = (int)statusCode;

        var payload = JsonSerializer.Serialize(new
        {
            status = (int)statusCode,
            title,
            errors
        });

        return context.Response.WriteAsync(payload);
    }
}