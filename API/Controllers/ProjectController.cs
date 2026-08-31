using Application.Features.Projects.Commands.Create;
using Application.Features.Projects.Queries.GetProjects;
using MediatR;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ProjectController(ISender sender) : ControllerBase
    {
        // get all projects
        [HttpGet]
        public async Task<IActionResult> GetAll()
        {
            var result = await sender.Send(new GetProjectsQuery());
            return Ok(result);
        }

        [HttpPost]
        public async Task<IActionResult> Create(CreateProjectCommand request)
        {
            var result = await sender.Send(request);
            return Ok(result);
        }

    }
}
