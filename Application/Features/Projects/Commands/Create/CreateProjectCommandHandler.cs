using Application.Interfaces;
using Domain.Entities;
using MediatR;

namespace Application.Features.Projects.Commands.Create;

public class CreateProjectCommandHandler : IRequestHandler<CreateProjectCommand , bool>
{
    private readonly IUnitOfWork _unitOfWork;

    public CreateProjectCommandHandler(IUnitOfWork unitOfWork)
    {
        _unitOfWork = unitOfWork;
    }

    public async Task<bool> Handle(CreateProjectCommand request, CancellationToken cancellationToken)
    {

        var project = new Project()
        {
            Name = request.Name,
            Description = request.Description,
            Status = request.Status
        };
        
        await _unitOfWork.ProjectRepository.CreateAsync(project , cancellationToken);
        return await _unitOfWork.SaveChangesAsync(cancellationToken) > 0;
 

    }
}