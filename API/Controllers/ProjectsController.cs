using Application.Common.Identity;
using Application.Common.ResultPattern;
using Application.Features.Projects.Commands.Create;
using Application.Features.Projects.Commands.Delete;
using Application.Features.Projects.Commands.Update;
using Application.Features.Projects.Queries.GetProjectById;
using Application.Features.Projects.Queries.GetProjects;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace API.Controllers;

[ApiController]
[Route("api/[controller]")]
[Authorize(Policy = "AnyAuthenticatedUser")]
public class ProjectsController(ISender sender , IUserService currentUser) : ApiBaseController
{
    [HttpPost]
    public async Task<IActionResult> Create(CreateProjectCommand command, CancellationToken cancellationToken)
    {
        var result = await sender.Send(command, cancellationToken);
        if (!result.IsSuccess)
            return ToProblem(result.Errors);

        var id = result.Value;
        return CreatedAtAction(nameof(GetById), new { id }, new { id });
    }

    [HttpGet]
    public async Task<IActionResult> GetAll(CancellationToken cancellationToken)
    {
        var userId = currentUser.UserId;
        if (userId == null)
            return ToProblem([
                new Error("User.Unauthorized",
                    "Could not resolve the current user.",
                    ErrorType.Unauthorized)
            ]);
        var result = await sender.Send(new GetProjectsQuery(userId), cancellationToken);
        if (!result.IsSuccess)
            return ToProblem(result.Errors);

        return Ok(result.Value);
    }

    [HttpGet("{id:int}")]
    public async Task<IActionResult> GetById(int id, CancellationToken cancellationToken)
    {
        var result = await sender.Send(new GetProjectByIdQuery(id), cancellationToken);
        if (!result.IsSuccess)
            return ToProblem(result.Errors);

        return Ok(result.Value);
    }

    [HttpPut("{id:int}")]
    public async Task<IActionResult> Update(int id, [FromBody] UpdateProjectCommand command, CancellationToken cancellationToken)
    {
        if (id != command.Id)
            return BadRequest("Route id and body id must match.");

        var result = await sender.Send(command, cancellationToken);
        if (!result.IsSuccess)
            return ToProblem(result.Errors);

        return NoContent();
    }

    [HttpDelete("{id:int}")]
    public async Task<IActionResult> Delete(int id, CancellationToken cancellationToken)
    {
        var result = await sender.Send(new DeleteProjectCommand(id), cancellationToken);
        if (!result.IsSuccess)
            return ToProblem(result.Errors);

        return NoContent();
    }
}