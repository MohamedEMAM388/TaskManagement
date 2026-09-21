using FluentValidation;

namespace Application.Features.Projects.Queries.GetProjectById;

public class GetProjectByIdQueryValidator : AbstractValidator<GetProjectByIdQuery>
{
    public GetProjectByIdQueryValidator()
    {
        RuleFor(x => x.Id).GreaterThan(0);
    }
}
