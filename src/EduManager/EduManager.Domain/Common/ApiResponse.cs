namespace EduManager.Domain.Common;

public class ApiResponse<T>
{
    public bool IsSuccess { get; set; }
    public string Message { get; set; } = string.Empty;
    public T? Data { get; set; }
    public List<string>? Errors { get; set; } = [];
    public string? ErrorCode { get; set; }

    public static ApiResponse<T> Success(T? data, string message = "Success") =>
        new() { IsSuccess = true, Data = data, Message = message };

    public static ApiResponse<T> Failure(string message, List<string>? error = null) =>
        new() { IsSuccess = false, Message = message, Errors = error ?? [], ErrorCode = ErrorCodeGenerator.Generate() };

}
