using Application.Interfaces;
using Domain.Entities;
using Domain.Entities.Enums;
using MediatR;

namespace Application.Features.Projects.Commands.Create;

public class CreateProjectCommandHandler(IUnitOfWork unitOfWork) : IRequestHandler<CreateProjectCommand , int>
{


    public async Task<int> Handle(CreateProjectCommand request, CancellationToken cancellationToken)
    {
                                   // Business Rules //
        // 1 => project has a unique name 
        var exists = await unitOfWork.ProjectRepository.HasNameAsync(request.Name);
        if (exists)
              throw new InvalidOperationException
                  ($"Project With Name {request.Name} already exists");
        
        // 2 => start from pending state
        var project = new Project()
        {
            Name = request.Name,
            Description = request.Description,
            Status = ProjectStatus.Pending
        };
        
         await unitOfWork.ProjectRepository.CreateAsync(project , cancellationToken);
         await unitOfWork.SaveChangesAsync(cancellationToken) ;
         return project.Id;
    }
}