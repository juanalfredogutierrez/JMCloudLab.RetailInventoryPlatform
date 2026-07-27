namespace BuildingBlocks.Application;

public class Result
{
    public bool IsSuccess { get; }

    public bool IsFailure => !IsSuccess;

    public IReadOnlyCollection<Error> Errors { get; }

    public Error FirstError => Errors.FirstOrDefault();

    protected Result(bool isSuccess,IEnumerable<Error> errors = null)
    {
        IsSuccess = isSuccess;
        Errors = errors?.ToList() ?? [];
    }

    public static Result Success() => new(true);

    public static Result Failure(params Error[] errors)
        => new(false, errors);
}