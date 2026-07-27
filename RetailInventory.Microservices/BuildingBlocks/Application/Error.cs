namespace BuildingBlocks.Application;

public sealed class Error
{
    public string Code { get; init; }
    public string Message { get; init; }
    public Error(string code, string message)
    {
        Code = code;
        Message = message;
    }
}