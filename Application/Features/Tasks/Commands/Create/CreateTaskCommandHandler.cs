using Application.Common.Identity;
using Application.Common.ResultPattern;
using Application.Contracts;
using Application.Features.Projects;
using DomainTask = Domain.Entities.Task;
using MediatR;

namespace Application.Features.Tasks.Commands.Create;

public class CreateTaskCommandHandler(IUnitOfWork unitOfWork , 
    IUserService userService) : IRequestHandler<CreateTaskCommand, Result<int>>
{
    public async Task<Result<int>> Handle(CreateTaskCommand request, CancellationToken cancellationToken)
    {

        var project = await unitOfWork.ProjectRepository.GetByIdAsync(request.ProjectId, cancellationToken);
        if (project is null)
            return Result<int>.Fail(Error.NotFound("Project.NotFound",
                $"Project with id '{request.ProjectId}' was not found."));

        if (!project.CanAcceptTasks)
            return Result<int>.Fail(Error.Conflict("Project.NotAcceptingTasks",
                $"Cannot add tasks to a project with status '{project.Status}'."));

        // get user 
        var userId = userService.UserId;
        if (userId is null)
            return Result<int>.Fail(Error.Validation(
                "User.NotAuthenticated", "User is not authenticated"));
        
        // Only the project owner can create tasks inside it
        if (project.CreatedByUserId != userId)
            return Result<int>.Fail(Error.Forbidden("Project.Forbidden",
                "You are not allowed to add tasks to this project."));
        
        var task = new DomainTask
        {
            Title = request.Title,
            Description = request.Description,
            DueDate = request.DueDate,
            ProjectId = project.Id,
            CreatedByUserId =  userId,
        };

        await unitOfWork.TaskRepository.CreateTaskAsync(task, cancellationToken);
        await unitOfWork.SaveChangesAsync(cancellationToken);

        return Result<int>.Ok(task.Id);
    }
}
