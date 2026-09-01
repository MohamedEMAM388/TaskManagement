using Application.Features.Tasks.Commands.Create;
using MediatR;
using Microsoft.AspNetCore.Mvc;

namespace API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class TaskController(ISender sender) : ControllerBase
    {
        // create task
        [HttpPost("CreateTask")]
        public async Task<IActionResult> CreateTask(CreateTaskCommand request)
        {
            var result = await sender.Send(request);
            return Ok(result);
        }
    }
}
