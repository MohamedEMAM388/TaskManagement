using System.Reflection;
using Application.Common.ResultPattern;
using FluentValidation;
using MediatR;

namespace Application.Behaviors;

public class ValidationBehavior<TRequest , TResponse> : IPipelineBehavior<TRequest , TResponse>
where TRequest : notnull
where TResponse : Result
{
    private readonly IEnumerable<IValidator<TRequest>> _validators;

    public ValidationBehavior(IEnumerable<IValidator<TRequest>> validators)
    {
        _validators = validators;
    }

    public async Task<TResponse> Handle(TRequest request, RequestHandlerDelegate<TResponse> next, CancellationToken cancellationToken)
    {
        if (!_validators.Any())
            return await next(cancellationToken);

        var context = new ValidationContext<TRequest>(request);
        var validationResults = await Task.WhenAll(
            _validators.
                Select(v => v.ValidateAsync(context, cancellationToken ))
            );

        var errors = validationResults
            .SelectMany(r => r.Errors)
            .Where(e => e != null)
            .Select(e => Error.Validation(e.PropertyName, e.ErrorMessage))
            .ToList();

        if (errors.Count != 0)
            return CreateFailedResult(errors);

        return await next(cancellationToken);
    }

    // TResponse is either Result or Result<T>, so we build the matching failed result
    private static TResponse CreateFailedResult(IReadOnlyList<Error> errors)
    {
        if (typeof(TResponse) == typeof(Result))
            return (TResponse)Result.Fail(errors);

        // Result<T>.Fail(IReadOnlyList<Error>) — found by reflection because T is only known at runtime
        var failMethod = typeof(TResponse).GetMethod(
            nameof(Result.Fail),
            BindingFlags.Public | BindingFlags.Static,
            binder: null,
            types: new[] { typeof(IReadOnlyList<Error>) },
            modifiers: null)!;

        return (TResponse)failMethod.Invoke(null, new object[] { errors })!;
    }
}
