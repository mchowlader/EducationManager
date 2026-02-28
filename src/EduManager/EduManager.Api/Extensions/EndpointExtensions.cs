using EduManager.Api.Endpoints;
using System.Reflection;

namespace EduManager.Api.Extensions;

public static class EndpointExtensions
{
    public static void MapAllEndpoints(this WebApplication app)
    {
        var endpointsTypes = typeof(Program).Assembly
            .GetTypes()
            .Where(t => t.IsClass && !t.IsAbstract && typeof(IEndpoints).IsAssignableFrom(t));

        foreach(var endpointsType in endpointsTypes)
        {
            var methodInfo = endpointsType.GetMethod(nameof(IEndpoints.MapEndpoints), 
                BindingFlags.Public | BindingFlags.Static);

            methodInfo?.Invoke(null, [app]);
        }
    }
}