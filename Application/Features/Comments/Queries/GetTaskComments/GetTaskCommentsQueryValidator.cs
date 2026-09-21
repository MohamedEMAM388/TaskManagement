using FluentValidation;

namespace Application.Features.Comments.Queries.GetTaskComments;

public class GetTaskCommentsQueryValidator : AbstractValidator<GetTaskCommentsQuery>
{
    public GetTaskCommentsQueryValidator()
    {
        RuleFor(x => x.TaskId).GreaterThan(0);
    }
}
