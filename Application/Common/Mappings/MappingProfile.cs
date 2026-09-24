using Application.Features.Comments.Dtos;
using Application.Features.Projects.DTOS;
using Application.Features.Tasks.DTOs;
using AutoMapper;
using Domain.Entities;
using DomainTask = Domain.Entities.Task;

namespace Application.Common.Mappings;

public class MappingProfile : Profile
{
    public MappingProfile()
    {
        CreateMap<Project, ProjectDto>();
        CreateMap<DomainTask, TaskDto>();
        CreateMap<Comment, CommentDto>();
    }
}
