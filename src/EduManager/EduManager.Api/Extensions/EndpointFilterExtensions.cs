using EduManager.Api.Filters;

namespace EduManager.Api.Extensions;

public static class EndpointFilterExtensions
{
    public static RouteHandlerBuilder WithValidation<T>(this RouteHandlerBuilder builder)
        where T : class
    {
        return builder.AddEndpointFilter<ValidationFilter<T>>();
    }
}
