namespace EduManager.Domain.Common;

public class ApiResponse<T>
{
    public bool isSuccess { get; set; }
    public string Message { get; set; } = string.Empty;
    public T? Data { get; set; }
    public List<string>? Errors { get; set; } = [];

    public static ApiResponse<T> Success(T? data, string message = "Success") =>
        new() { isSuccess = true, Data = data, Message = message };

    public static ApiResponse<T> Failure(string message, List<string>? error = null) =>
        new() { isSuccess = false, Message = message, Errors = error ?? [] };

}
