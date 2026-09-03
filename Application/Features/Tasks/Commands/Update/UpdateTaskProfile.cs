using AutoMapper;

namespace Application.Features.Tasks.Commands.update;

public class UpdateTaskProfile : Profile
{
    public UpdateTaskProfile()
    {
        CreateMap<UpdateTaskCommand, Task>()
            .ForMember(dest => dest.Id,
                opt => opt.Ignore());


    }
}