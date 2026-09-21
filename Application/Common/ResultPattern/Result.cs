namespace Application.Common.ResultPattern;

public class Result
{
    public bool IsSuccess { get;}
    
    public IReadOnlyList<Error>  Errors { get;}
    
    protected Result(bool isSuccess, IReadOnlyList<Error> errors)
    {
        IsSuccess = isSuccess;
        Errors = errors;
    }
    
    // factory methods
    
    // ok 
    public static Result Ok() => new Result(true, Array.Empty<Error>());
    
    // fail with one error
    public static Result Fail(Error error) => new Result(false, [error]);
    
    // fail with Multi Error
    public static Result Fail(IReadOnlyList<Error> errors) => new Result(false, errors);
}

public class Result<TValue> : Result
{
    private readonly TValue? _value;
    public TValue Value => IsSuccess ? _value! : 
        throw new InvalidOperationException("The result is not successful.");
    
    
    public Result(TValue value) : base(true, Array.Empty<Error>())
    {
        _value = value;
    }

    public Result(Error error) : base(false, [error])
    {
        _value = default;
    }
    
    public Result(IReadOnlyList<Error> errors) : base(false, errors)
    {
        _value = default;
    }




    // ok
    public static Result<TValue> Ok(TValue value) => new Result<TValue>(value);
    
    // fail with one error
    public static Result<TValue> Fail(Error error) => new Result<TValue>(error);
    
    // fail with multi error
    public static Result<TValue> Fail(IReadOnlyList<Error> errors) => new Result<TValue>(errors);
}