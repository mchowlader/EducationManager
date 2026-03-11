using Asp.Versioning;
using EduManager.Api.Extensions;
using EduManager.Api.Metadata;
using EduManager.Application.Features.Base.Commands;
using EduManager.Application.Features.Base.Queries;
using EduManager.Domain.Common;
using EduManager.Domain.Constants;
using MediatR;
using Microsoft.AspNetCore.Mvc;
using OpenTelemetry.Trace;

namespace EduManager.Api.Endpoints;

public abstract class BaseEndPoints<TEntity, TCreateDto, TUpdateDto, TResponseDto>
    where TEntity : BaseEntity, new()
    where TCreateDto : class
    where TUpdateDto : class
    where TResponseDto : class
{
    protected static string EntityName => typeof(TEntity).Name;
    protected static string Route => ToKebabCase(typeof(TEntity).Name);
    
    protected virtual double[] SupportedVersions => [1.0];
    protected virtual double[] DerecatedVersions => [];

    private static string ViewPermission => Permissions.For(EntityName, Permissions.View);
    private static string CreatePermission => Permissions.For(EntityName, Permissions.Create);
    private static string UpdatePermission => Permissions.For(EntityName, Permissions.Update);
    private static string DeletePermission => Permissions.For(EntityName, Permissions.Delete);

    protected static RouteGroupBuilder MapBaseEndPoints(IEndpointRouteBuilder app, double[] supportedVersions, double[] derecatedVersions)
    {
        var versionBuilder = app.NewApiVersionSet()
            .ReportApiVersions();

        foreach(var version in supportedVersions)
            versionBuilder.HasApiVersion(new ApiVersion(version));

        foreach(var version in derecatedVersions)
            versionBuilder.HasDeprecatedApiVersion(new ApiVersion(version));

        var versionSet = versionBuilder.Build();

        var group = app.MapGroup($"/api/v/{{version:apiVersion}}/{Route}")
            .WithApiVersionSet(versionSet)
            .WithTags(EntityName);

        foreach(var version in supportedVersions)
        {
            var isDeprecated = derecatedVersions.Contains(version);

            var getAll = group.MapGet("/", GetAllHandler)
                .WithName($"GetAll{EntityName}V{version}")
                .WithSummary($"Get all {EntityName}")
                .MapToApiVersion(version)
                .RequireAuthorization(ViewPermission)
                .WithValidation<TCreateDto>()
                .Produces<ApiResponse<IEnumerable<TResponseDto>>>(StatusCodes.Status200OK)
                .Produces(StatusCodes.Status401Unauthorized)
                .Produces(StatusCodes.Status403Forbidden)
                .Produces(StatusCodes.Status500InternalServerError);

            var getById = group.MapGet("/{id:long}", GetByIdHandler)
                .WithName($"Get{EntityName}ByIdV{version}")
                .WithSummary($"Get {EntityName} by ID")
                .MapToApiVersion(version)
                .RequireAuthorization(ViewPermission)
                .Produces<ApiResponse<TResponseDto>>(StatusCodes.Status200OK)
                .Produces(StatusCodes.Status404NotFound)
                .Produces(StatusCodes.Status401Unauthorized)
                .Produces(StatusCodes.Status403Forbidden)
                .Produces(StatusCodes.Status500InternalServerError); ;

            var create = group.MapPost("/", CreateHandler)
                .WithName($"Create{EntityName}V{version}")
                .WithDescription($"Create {EntityName}")
                .MapToApiVersion(version)
                .RequireAuthorization(CreatePermission)
                .Produces<ApiResponse<TResponseDto>>(StatusCodes.Status201Created)
                .Produces(StatusCodes.Status400BadRequest)
                .Produces(StatusCodes.Status401Unauthorized)
                .Produces(StatusCodes.Status403Forbidden)
                .Produces(StatusCodes.Status500InternalServerError);

            var update = group.MapPatch("/{id:long}", UpdateHandler)
                .WithSummary($"Update {EntityName}")
                .WithDescription($"Update {EntityName}")
                .MapToApiVersion(version)
                .RequireAuthorization(UpdatePermission)
                .WithValidation<TUpdateDto>()
                .Produces<ApiResponse<TResponseDto>>(StatusCodes.Status200OK)
                .Produces(StatusCodes.Status400BadRequest)
                .Produces(StatusCodes.Status404NotFound)
                .Produces(StatusCodes.Status401Unauthorized)
                .Produces(StatusCodes.Status403Forbidden)
                .Produces(StatusCodes.Status500InternalServerError);

            var delete = group.MapDelete("/{id:long}", DeleteHandler)
                .WithSummary($"Delete{EntityName}V{version}")
                .WithDescription($"Delete {EntityName}")
                .MapToApiVersion(version)
                .RequireAuthorization(DeletePermission)
                .Produces(StatusCodes.Status204NoContent)
                .Produces(StatusCodes.Status404NotFound)
                .Produces(StatusCodes.Status401Unauthorized)
                .Produces(StatusCodes.Status403Forbidden)
                .Produces(StatusCodes.Status500InternalServerError);


            if (isDeprecated)
            {
                getAll.Add(builder => builder.Metadata.Add(new EndpointDeprecatedMetadata()));
                create.Add(builder => builder.Metadata.Add(new EndpointDeprecatedMetadata()));
                update.Add(builder => builder.Metadata.Add(new EndpointDeprecatedMetadata()));
                delete.Add(builder => builder.Metadata.Add(new EndpointDeprecatedMetadata()));
                getById.Add(builder => builder.Metadata.Add(new EndpointDeprecatedMetadata()));
            }

        }

        return group;
    }

    private static async Task<IResult> GetAllHandler(IMediator mediator
        , CancellationToken cancellationToken
        , int pageNumber = 1
        , int pageSize = 10)
    {
        var result = await mediator.Send(
            new BaseGetAllQuery<TEntity, TResponseDto>(pageNumber, pageSize), cancellationToken);

        return result.IsSuccess
            ? Results.Ok(ApiResponse<IEnumerable<TResponseDto>>.Success(result.Data))
            : Results.BadRequest(ApiResponse<TResponseDto>.Failure(result.Message!));
    }

    private static async Task<IResult> GetByIdHandler(long id
        , IMediator mediator
        , CancellationToken cancellationToken)
    {
        var result = await mediator.Send(
            new BaseGetByIdQuery<TEntity, TResponseDto>(id), cancellationToken);

        return result.IsSuccess
            ? Results.Ok(ApiResponse<TResponseDto>.Success(result.Data))
            : Results.NotFound(ApiResponse<TResponseDto>.Failure(result.Message!));
    }

    private static async Task<IResult> CreateHandler(TCreateDto dto
        , IMediator mediator
        , CancellationToken cancellationToken)
    {
        var result = await mediator.Send(
            new BaseCreateCommand<TEntity, TCreateDto, TResponseDto>(dto));

        return result.IsSuccess
            ? Results.Created($"/api/{Route}/{result.Data}",
                ApiResponse<TResponseDto>.Success(result.Data))
            : Results.BadRequest(ApiResponse<TResponseDto>.Failure(result.Message!));
    }

    private static async Task<IResult> UpdateHandler(long id
        , TUpdateDto dto
        , IMediator mediator
        , CancellationToken cancellationToken)
    {
        var result = await mediator.Send(
            new BaseUpdateCommand<TEntity, TUpdateDto, TResponseDto>(id, dto), cancellationToken);

        return result.IsSuccess
            ? Results.Ok(ApiResponse<TResponseDto>.Success(result.Data))
            : Results.BadRequest(ApiResponse<TResponseDto>.Failure(result.Message!));
    }

    private static async Task<IResult> DeleteHandler(long id
        , IMediator mediator
        , CancellationToken cancellationToken)
    {
        var result = await mediator.Send(
            new BaseDeleteCommand<TEntity>(id), cancellationToken);

        return result.IsSuccess
            ? Results.NoContent()
            : Results.BadRequest(ApiResponse<object>.Failure(result.Message!));
    }

    private static string ToKebabCase(string value) =>
        string.Concat(value.Select((x, i) =>
        i > 0 && char.IsUpper(x) ? "-" + x : x.ToString())).ToLower();
}
