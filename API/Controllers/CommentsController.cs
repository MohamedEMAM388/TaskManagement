using Application.Features.Comments.Commands.Add;
using Application.Features.Comments.Commands.Delete;
using Application.Features.Comments.Queries.GetTaskComments;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace API.Controllers;

[ApiController]
[Route("api/[controller]")]
[Authorize(Policy = "AnyAuthenticatedUser")]
public class CommentsController(ISender sender) : ApiBaseController
{
    [HttpPost]
    public async Task<IActionResult> Add(AddCommentCommand command, CancellationToken cancellationToken)
    {
        var result = await sender.Send(command, cancellationToken);
        if (!result.IsSuccess)
            return ToProblem(result.Errors);

        var id = result.Value;
        return CreatedAtAction(nameof(GetTaskComments), new { taskId = command.TaskId }, new { id });
    }

    [HttpGet("task/{taskId:int}")]
    public async Task<IActionResult> GetTaskComments(int taskId, CancellationToken cancellationToken)
    {
        var result = await sender.Send(new GetTaskCommentsQuery(taskId), cancellationToken);
        if (!result.IsSuccess)
            return ToProblem(result.Errors);

        return Ok(result.Value);
    }

    [HttpDelete("{id:int}")]
    public async Task<IActionResult> Delete(int id, CancellationToken cancellationToken)
    {
        var result = await sender.Send(new DeleteCommentCommand(id), cancellationToken);
        if (!result.IsSuccess)
            return ToProblem(result.Errors);

        return NoContent();
    }
}