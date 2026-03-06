using Asp.Versioning.ApiExplorer;
using Microsoft.OpenApi;

namespace EduManager.Api.Extensions;

public static class SwaggerExtension
{
    public static IServiceCollection AddSwaggerWithVersioning(
        this IServiceCollection services,
        IHostEnvironment environment)  
    {
        services.AddSwaggerGen(options =>
        {
            if (environment.IsDevelopment()) 
            {
                options.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
                {
                    Name = "Authorization",
                    Type = SecuritySchemeType.Http,
                    Scheme = "bearer",
                    BearerFormat = "JWT",
                    In = ParameterLocation.Header,
                    Description = "Give the JWT token — Bearer {token}"
                });

                options.AddSecurityRequirement(document => new OpenApiSecurityRequirement
                {
                    [new OpenApiSecuritySchemeReference("Bearer", document)] = []
                });
            }
        });

        services.ConfigureOptions<SwaggerVersioningOptions>();

        return services;
    }

    public static WebApplication UseSwaggerWithVersioning(
        this WebApplication app,  // ← WebApplicationBuilder → WebApplication
        IApiVersionDescriptionProvider provider)
    {
        app.UseSwagger();
        app.UseSwaggerUI(options =>
        {
            foreach (var description in provider.ApiVersionDescriptions)
            {
                var url = $"/swagger/{description.GroupName}/swagger.json";
                var name = description.IsDeprecated
                    ? $"{description.GroupName} (Deprecated)"
                    : description.GroupName.ToUpperInvariant();

                options.SwaggerEndpoint(url, name);
            }

            options.RoutePrefix = "swagger";
        });

        return app;
    }
}