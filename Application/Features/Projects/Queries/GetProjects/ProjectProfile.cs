using AutoMapper;
using Domain.Entities;

namespace Application.Features.Projects.Queries.GetProjects;

public class ProjectProfile : Profile
{

    public ProjectProfile()
    {
        
        CreateMap<Project, ProjectDto>()
            .ForMember( p => p.StartAt , 
                dst =>
                    dst.MapFrom(opt => opt.CreateAt));
      
        
    }
    
}