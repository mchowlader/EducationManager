using Asp.Versioning;
using Asp.Versioning.ApiExplorer;
using EduManager.Api.Extensions;
using EduManager.Api.Middleware;
using EduManager.Application;
using EduManager.Domain.Common;
using EduManager.Infrastructure;
using NpgsqlTypes;
using Serilog;
using Serilog.Sinks.PostgreSQL;
using System.Threading.RateLimiting;

var builder = WebApplication.CreateBuilder(args);

var pgLogColumnOptions = new Dictionary<string, ColumnWriterBase>
{
    { "message",          new RenderedMessageColumnWriter() },
    { "message_template", new MessageTemplateColumnWriter() },
    { "level",            new LevelColumnWriter(true, NpgsqlDbType.Varchar) },
    { "raise_time",       new TimestampColumnWriter() },
    { "exception",        new ExceptionColumnWriter() },
    { "properties",       new LogEventSerializedColumnWriter() },
    { "ErrorCode",        new SinglePropertyColumnWriter("ErrorCode", PropertyWriteMethod.Raw, NpgsqlDbType.Text) }
};

Log.Logger = new LoggerConfiguration()
    .ReadFrom.Configuration(builder.Configuration)
    .WriteTo.PostgreSQL(
        connectionString: builder.Configuration.GetConnectionString("MasterDBConnection"),
        tableName: "Logs",
        columnOptions: pgLogColumnOptions,
        needAutoCreateTable: false,
        schemaName: "public")
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

builder.Services.AddRateLimiter(option =>
{
    option.RejectionStatusCode = StatusCodes.Status429TooManyRequests;
    option.GlobalLimiter = PartitionedRateLimiter.Create<HttpContext, string>(context =>
    {
        var clientIP = context.Connection.RemoteIpAddress?.ToString() ?? "unknow";

        return RateLimitPartition.GetFixedWindowLimiter(clientIP, _ =>
            new FixedWindowRateLimiterOptions
            {
                PermitLimit = 100,
                Window = TimeSpan.FromMinutes(1),
                QueueLimit = 0
            });
    });

    option.AddPolicy("strict", context =>
    {
        var clientIp = context.Connection.RemoteIpAddress?.ToString() ?? "unknown";
        return RateLimitPartition.GetFixedWindowLimiter(clientIp, _ =>
            new FixedWindowRateLimiterOptions
            {
                PermitLimit = 5,         
                Window = TimeSpan.FromMinutes(1),  
                QueueLimit = 0
            });
    });

    option.OnRejected = async(context, CancellationToken) =>
    {
        context.HttpContext.Response.StatusCode = StatusCodes.Status429TooManyRequests;
        context.HttpContext.Response.ContentType = "application/json";

        var response = ApiResponse<object>.Failure("Too many requests. Please try again later.");
        await context.HttpContext.Response.WriteAsJsonAsync(response);
    };
});

var app = builder.Build();

app.UseSerilogRequestLogging(); 
app.UseMiddleware<GlobalExceptionMiddleware>();
app.UseTenantMiddleware();
app.UseRateLimiter();
app.MapDefaultEndpoints();
app.UseHttpsRedirection();
app.UseAuthentication();
app.UseAuthorization();
app.MapAllEndpoints();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    var provider = app.Services.GetRequiredService<IApiVersionDescriptionProvider>();
    app.UseSwaggerWithVersioning(provider);
}
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
