using EduManager.Domain.Common;
using FluentValidation;

namespace EduManager.Api.Filters;

public class ValidationFilter<T>(IValidator<T> validator) : IEndpointFilter
    where T : class
{
    public async ValueTask<object?> InvokeAsync(EndpointFilterInvocationContext context, EndpointFilterDelegate next)
    {
        var argument = context.Arguments
            .OfType<T>()
            .FirstOrDefault();

        if (argument is null)
            return await next(context);

        var validationResult = await validator.ValidateAsync(argument);

        if(!validationResult.IsValid)
        {
            var errors = validationResult.Errors
                .Select(x => x.ErrorMessage)    
                .ToList();

            return Results.BadRequest(ApiResponse<object>.Failure("Validation failed", ErrorCodeGenerator.Generate(), errors));
        }

        return await next(context);
    }
}
