using Application.Common.Identity;
using Application.Common.ResultPattern;
using Application.Contracts;
using Domain.Entities;
using MediatR;

namespace Application.Features.Projects.Commands.Create;

public class CreateProjectCommandHandler(IUnitOfWork unitOfWork ,
    IUserService userService) : IRequestHandler<CreateProjectCommand, Result<int>>
{
    public async Task<Result<int>> Handle(CreateProjectCommand request, CancellationToken cancellationToken)
    {
        // get user that will create the project
        var userId = userService.UserId;
        if (userId is null)
            return Result<int>.Fail(Error.Unauthorized(
                "User.NotAuthenticated", "User is not authenticated"));
        
        var project = new Project
        {
            Name = request.Name,
            Description = request.Description,
            Status = request.Status,
            CreatedByUserId = userId,
            
        };

        await unitOfWork.ProjectRepository.CreateAsync(project, cancellationToken);
        await unitOfWork.SaveChangesAsync(cancellationToken);

        return Result<int>.Ok(project.Id);
    }
}
