using System.Text.Json.Serialization;

namespace EduManager.Domain.Common;

public class ApiResponse<T>
{
    public bool IsSuccess { get; set; }
    public string Message { get; set; } = string.Empty;

    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingDefault)]
    public T? Data { get; set; }

    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
    public List<string>? Errors { get; set; }

    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
    public string? ErrorCode { get; set; }

    public static ApiResponse<T> Success(T? data, string message = "Success") =>
        new() { IsSuccess = true, Message = message, Data = data, };

    public static ApiResponse<T> Failure(string message, string? errorCode, List<string>? errors = null) =>
        new() { IsSuccess = false, Message = message, ErrorCode = errorCode, Errors = errors ?? [] };

}
