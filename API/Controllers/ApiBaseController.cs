using Application.Common.ResultPattern;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ApiBaseController : ControllerBase
    {
        protected static ActionResult<T> ToActionResult<T>(Result<T> result)
        {
            return result.IsSuccess
                ? new OkObjectResult(result.Value)
                : ToProblem(result.Errors);


        }
        
        protected static ActionResult ToActionResult(Result result)
        {
            return result.IsSuccess
                ? new OkResult()
                : ToProblem(result.Errors);


        }
        
        // handle error
        protected static ObjectResult ToProblem(IReadOnlyList<Error> errors)
        {
            if (errors.Count == 0)
            {
                return new ObjectResult(new ProblemDetails
                {
                    Status = StatusCodes.Status500InternalServerError,
                    Title = "Unknown error",
                    Detail = "An unexpected error occurred and no error details were provided."
                })
                {
                    StatusCode = StatusCodes.Status500InternalServerError
                };
            }
            var firstError = errors[0];
            var statusCode = firstError.ErrorType switch
            {
                ErrorType.NotFound => StatusCodes.Status404NotFound,
                ErrorType.Validation => StatusCodes.Status400BadRequest,
                ErrorType.Conflict => StatusCodes.Status409Conflict,
                ErrorType.Unauthorized => StatusCodes.Status401Unauthorized,
                ErrorType.InvalidCredentials => StatusCodes.Status401Unauthorized,
                ErrorType.Forbidden => StatusCodes.Status403Forbidden,
                _ => StatusCodes.Status500InternalServerError
            };

            var problem = new ProblemDetails()
            {
                Status = statusCode,
                Title = firstError.Code,
                Detail = firstError.Description,
                Extensions = { ["errors"] = errors }
            };
            return new ObjectResult(problem)
            {
                StatusCode = statusCode
            };
        }
    }
}
