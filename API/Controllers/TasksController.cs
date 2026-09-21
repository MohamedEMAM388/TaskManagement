using API.Extensions;
using Application.Features.Tasks.Commands.Create;
using Application.Features.Tasks.Commands.Delete;
using Application.Features.Tasks.Commands.Update;
using Application.Features.Tasks.Queries.GetTaskById;
using Application.Features.Tasks.Queries.GetTasks;
using Application.Features.Tasks.Commands.UpdateStatus;
using MediatR;
using Microsoft.AspNetCore.Mvc;
using TaskStatus = Domain.Entities.Enums.TaskStatus;

namespace API.Controllers;

[ApiController]
[Route("api/[controller]")]
public class TasksController(ISender sender) : ControllerBase
{
    [HttpPost]
    public async Task<IActionResult> Create(CreateTaskCommand command, CancellationToken cancellationToken)
    {
        var result = await sender.Send(command, cancellationToken);
        if (!result.IsSuccess)
            return result.ToProblem();

        var id = result.Value;
        return CreatedAtAction(nameof(GetById), new { id }, new { id });
    }

    [HttpGet]
    public async Task<IActionResult> GetAll(CancellationToken cancellationToken)
    {
        var result = await sender.Send(new GetTasksQuery(), cancellationToken);
        if (!result.IsSuccess)
            return result.ToProblem();

        return Ok(result.Value);
    }

    [HttpGet("{id:int}")]
    public async Task<IActionResult> GetById(int id, CancellationToken cancellationToken)
    {
        var result = await sender.Send(new GetTaskByIdQuery(id), cancellationToken);
        if (!result.IsSuccess)
            return result.ToProblem();

        return Ok(result.Value);
    }

    [HttpPut("{id:int}")]
    public async Task<IActionResult> Update(int id, [FromBody] UpdateTaskCommand command, CancellationToken cancellationToken)
    {
        if (id != command.Id)
            return BadRequest("Route id and body id must match.");

        var result = await sender.Send(command, cancellationToken);
        if (!result.IsSuccess)
            return result.ToProblem();

        return NoContent();
    }

    [HttpDelete("{id:int}")]
    public async Task<IActionResult> Delete(int id, CancellationToken cancellationToken)
    {
        var result = await sender.Send(new DeleteTaskCommand(id), cancellationToken);
        if (!result.IsSuccess)
            return result.ToProblem();

        return NoContent();
    }
    
    [HttpPatch("{id:int}/status")]
    public async Task<ActionResult<TaskStatus>> UpdateStatus(
        int id,
        [FromBody] TaskStatus status,
        CancellationToken cancellationToken)
    {
        var result = await sender.Send(new UpdateStatusCommand(id, status), cancellationToken);
        if (!result.IsSuccess)
            return result.ToProblem();

        return Ok(result.Value);
    }
}
