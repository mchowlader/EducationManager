using EduManager.Domain.Common;
using EduManager.Domain.Exceptions;

namespace EduManager.Api.Middleware;

public class GlobalExceptionMiddleware(RequestDelegate next, ILogger<GlobalExceptionMiddleware> logger)
{
    private readonly RequestDelegate _next = next;
    private readonly ILogger<GlobalExceptionMiddleware> _logger = logger;

    public async Task InvokeAsync(HttpContext context)
    {
        try
        {
            await _next(context);
        }
        catch (TenantContextNotFoundException)
        {
            context.Response.StatusCode = 404;
            context.Response.ContentType = "application/json";

            var response = new ApiResponse<Object>()
            {
                IsSuccess = false,
                Message = "Tenant not found."
            };

            await context.Response.WriteAsJsonAsync(response);
        }
        catch (Exception ex)
        {
            var errorCode = ErrorCodeGenerator.Generate();

            _logger.LogError(ex
                , "ErrorCode: {ErrorCode} — Unhandled exception. Path: {Path}, Method: {Method}, Query: {Query}"
                , errorCode
                , context.Request.Path
                , context.Request.Method
                , context.Request.QueryString);

            context.Response.StatusCode = 500;
            context.Response.ContentType = "application/json";

            var response = new ApiResponse<Object>()
            {
                IsSuccess = false,
                Message = "Internal Server  Error.",
                ErrorCode = errorCode
            };

            await context.Response.WriteAsJsonAsync(response);
        }
    }
}
