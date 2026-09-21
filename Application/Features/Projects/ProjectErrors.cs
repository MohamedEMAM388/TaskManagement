using Application.Common.ResultPattern;

namespace Application.Features.Projects;

public static class ProjectErrors
{
    public static Error NotFound(int id) =>
        Error.NotFound("Project.NotFound", $"Project with id '{id}' was not found.");
}
