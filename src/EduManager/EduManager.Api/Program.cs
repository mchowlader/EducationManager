using Asp.Versioning;
using Asp.Versioning.ApiExplorer;
using EduManager.Api.Extensions;
using EduManager.Api.Middleware;
using EduManager.Application;
using EduManager.Infrastructure;
using Serilog;
using Serilog.Sinks.MSSqlServer;

var builder = WebApplication.CreateBuilder(args);

Log.Logger = new LoggerConfiguration()
    .ReadFrom.Configuration(builder.Configuration)
    .CreateLogger();

builder.Host.UseSerilog();

builder.Services
.AddApiVersioning(opt => 
{
    opt.DefaultApiVersion = new ApiVersion(1, 0);
    opt.AssumeDefaultVersionWhenUnspecified = true;
    opt.ReportApiVersions = true;
})
.AddApiExplorer(opt =>
{
    opt.GroupNameFormat = "'v'VVV";
    opt.SubstituteApiVersionInUrl = true;
});

builder.Services.AddSwaggerWithVersioning(builder.Environment);
builder.Services.AddApplication();
builder.Services.AddInfrastructure(builder.Configuration);
builder.AddServiceDefaults();

var app = builder.Build();
app.UseSerilogRequestLogging();
app.UseMiddleware<GlobalExceptionMiddleware>();
app.MapDefaultEndpoints();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    var provider = app.Services.GetRequiredService<IApiVersionDescriptionProvider>();
    app.UseSwaggerWithVersioning(provider);
}
app.UseHttpsRedirection();
app.UseAuthentication();
app.UseAuthorization();
app.MapAllEndpoints();
try
{
    app.Run();
}
catch (Exception ex)
{
    Log.Fatal(ex, "Application crashed.");
}
finally
{
    Log.CloseAndFlush();
}
