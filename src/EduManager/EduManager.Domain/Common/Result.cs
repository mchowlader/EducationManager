namespace EduManager.Domain.Common;

public interface IOperationResult
{
    bool IsSuccess { get; }
    string? Message { get; }
    string? ErrorCode { get; }
}

public abstract class ResultBase : IOperationResult
{
    public bool IsSuccess { get; protected init; }
    public string? Message { get; protected init; }
    public string? ErrorCode { get; protected init; }
    public List<string> Errors { get; protected init; } = [];
}
public class Result : ResultBase
{
    private Result() { }

    public static Result Success(string message) =>
        new() { IsSuccess = true, Message = message };

    public static Result Failure(string message, List<string>? errors = null) =>
        new()
        {
            IsSuccess = false,
            Message = message,
            Errors = errors ?? [],
            ErrorCode = ErrorCodeGenerator.Generate()
        };
}

public class Result<T> : IOperationResult
{
    public bool IsSuccess { get; set; }
    public T? Data { get; set; }
    public string? Message { get; set; }
    public string? ErrorCode { get; set; }
    public List<string> Errors { get; set; } = new();

    public static Result<T> Success(T? data, string message = "Success") =>
        new() { IsSuccess = true, Data = data, Message = message };

    public static Result<T> Failure(string message, List<string>? errors = null)
    {
        var errorCode = ErrorCodeGenerator.Generate();
        return new() { IsSuccess = false, Message = message, Errors = errors ?? [], ErrorCode = errorCode };
    }
}
