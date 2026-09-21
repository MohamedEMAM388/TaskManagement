using Application.Common.ResultPattern;

namespace Application.Features.Tasks;

public static class TaskErrors
{
    public static Error NotFound(int id) =>
        Error.NotFound("Task.NotFound", $"Task with id '{id}' was not found.");

    public static Error InvalidStatusTransition(string message) =>
        Error.Validation("Task.InvalidStatusTransition", message);
}
