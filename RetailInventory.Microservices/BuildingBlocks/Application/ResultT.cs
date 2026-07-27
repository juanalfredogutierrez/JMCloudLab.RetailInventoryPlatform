namespace BuildingBlocks.Application;

public class Result<T> : Result
{
    public T Value { get; }

    private Result(T value) : base(true)
    {
        Value = value;
    }

    private Result(IEnumerable<Error> errors): base(false, errors)
    {
    }

    public static Result<T> Success(T value) => new(value);

    public static new Result<T> Failure(params Error[] errors)
        => new(errors);

    public static implicit operator Result<T>(T value)
        => Success(value);
}