using Asp.Versioning.ApiExplorer;
using Microsoft.Extensions.Options;
using Microsoft.OpenApi;
using Swashbuckle.AspNetCore.SwaggerGen;

namespace EduManager.Api.Extensions;

public class SwaggerVersioningOptions(IApiVersionDescriptionProvider provider) 
        : IConfigureOptions<SwaggerGenOptions>
{
    private readonly IApiVersionDescriptionProvider _provider = provider;
    public void Configure(SwaggerGenOptions options)
    {
        foreach (var description in _provider.ApiVersionDescriptions)
        {
            options.SwaggerDoc(description.GroupName, new OpenApiInfo
            {
                Title = "EduManager API",
                Version = description.GroupName,
                Description = description.IsDeprecated
                    ? "⚠️ This version is deprecated."
                    : "EduManager API"
            });
        }
    }
}
