using EduManager.Domain.Common;
using MediatR;
using Microsoft.Extensions.Logging;

namespace EduManager.Application.Behaviors;

public class LoggingBehavior<TRequest, TResponse>(ILogger<LoggingBehavior<TRequest, TResponse>> logger)
    : IPipelineBehavior<TRequest, TResponse>
    where TRequest : IRequest<TResponse>
{
    public async Task<TResponse> Handle(
        TRequest request,
        RequestHandlerDelegate<TResponse> next,
        CancellationToken cancellationToken)
    {
        var response = await next(cancellationToken);

        if (response is IOperationResult { IsSuccess: false } result)
        {
            logger.LogWarning(
                "Business failure [{ErrorCode}] on {RequestType}: {Message}",
                result.ErrorCode,
                typeof(TRequest).Name,
                result.Message);
        }

        return response;
    }
}
