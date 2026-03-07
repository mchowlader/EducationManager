using Asp.Versioning;
using EduManager.Application.DTOs.Feature.TenantFeature;
using EduManager.Application.Features.TenantFeature.Commands;
using EduManager.Application.Features.TenantFeature.Queries;
using EduManager.Domain.Common;
using MediatR;
using Microsoft.AspNetCore.Http.HttpResults;

namespace EduManager.Api.Endpoints;

public class TenantEndpoints : IEndpoints
{
    public static void MapEndpoints(WebApplication app)
    {
        var versionSet = app.NewApiVersionSet()
            .HasApiVersion(new ApiVersion(1, 0))
            .ReportApiVersions()
            .Build();

        var group = app.MapGroup("/api/v/{version:apiVersion}/tenants")
            .WithApiVersionSet(versionSet)
            .WithTags("Tenants");

        group.MapPost("/", CreateTenantHandlerV1)
            .WithName("CreateTenantV1")
            .WithSummary("Create Tenant")
            .WithDescription("Creates a new tenant and returns the created resource")
            .MapToApiVersion(1, 0);

        group.MapPatch("/{id:long}", UpdateTenantHandlerV1)
            .WithName("UpdateTenantV1")
            .WithSummary("Update Tenant")
            .WithDescription("Update an exiting tenant and returns the updated resource")
            .MapToApiVersion(1, 0);

        group.MapGet("/", GetAllTenantHandlerV1)
           .RequireRateLimiting("strict")
           .WithName("GetAllTenantV1")
           .WithSummary("Gat All Tenants")
           .WithDescription("Returns a paginated list of all tenants. Use pageNumber and pageSize to control pagination.")
           .MapToApiVersion(1, 0);

        group.MapGet("/{id:long}", GetByIdTenantHandlerV1)
            .WithName("GetByIdTenantV1")
            .WithSummary("Gat a Tenant")
            .WithDescription("Returns a Tenant record")
            .MapToApiVersion(1, 0);

        group.MapDelete("/{id:long}", DeleteHandlerV1)
           .WithName("DeleteV1")
           .WithSummary("Delete a Tenant")
           .WithDescription("Delete a Tenant record")
           .MapToApiVersion(1, 0);
    }

    private static async Task<
    Results<
        Ok<ApiResponse<object>>,
        BadRequest<ApiResponse<object>>>> 
    DeleteHandlerV1(long id
    , IMediator mediator
    , CancellationToken cancellationToken)
    {
        var result = await mediator.Send(new DeleteTenantCommand(id), cancellationToken);

        return result.IsSuccess
            ? TypedResults.Ok(ApiResponse<object>.Success(null, result.Message ?? "Tenant delete successfully"))
            : TypedResults.BadRequest(ApiResponse<object>.Failure(result.Message!));
    }

    private static async Task<
    Results<
        Ok<ApiResponse<TenantResponseDto>>,
        BadRequest<ApiResponse<TenantResponseDto>>>> 
    GetByIdTenantHandlerV1(long id
    , IMediator mediator
    , CancellationToken cancellationToken)
    {
        var result = await mediator.Send(new GetByIdTenantQuery(id), cancellationToken);

        return result.IsSuccess
            ? TypedResults.Ok(ApiResponse<TenantResponseDto>.Success(result.Data))
            : TypedResults.BadRequest(ApiResponse<TenantResponseDto>.Failure(result.Message!));
    }

    private static async Task<
    Results<
        Ok<ApiResponse<IEnumerable<TenantResponseDto>>>,
        BadRequest<ApiResponse<TenantResponseDto>>>>
     GetAllTenantHandlerV1(
     IMediator mediator,
     CancellationToken cancellationToken,
     int pageNumber = 1,
     int pageSize = 10)
    {
        var result = await mediator.Send(
            new GetAllTenantQuery(pageNumber, pageSize), cancellationToken);

        return result.IsSuccess
            ? TypedResults.Ok(ApiResponse<IEnumerable<TenantResponseDto>>.Success(result.Data))
            : TypedResults.BadRequest(ApiResponse<TenantResponseDto>.Failure(result.Message!));
    }

    private static async Task<
    Results<
        Ok<ApiResponse<TenantResponseDto>>,
        BadRequest<ApiResponse<TenantResponseDto>>>> 
    UpdateTenantHandlerV1(long id, IMediator mediator
    , UpdateTenantDto command
    , CancellationToken cancellationToken )
    {
        var result = await mediator.Send(
            new UpdateTenantCommand(id, command), cancellationToken);

        return result.IsSuccess
            ? TypedResults.Ok(ApiResponse<TenantResponseDto>.Success(result.Data))
            : TypedResults.BadRequest(ApiResponse<TenantResponseDto>.Failure(result.Message!));
    }

    private static async Task<
    Results<
        Created<ApiResponse<TenantResponseDto>>, 
        BadRequest<ApiResponse<TenantResponseDto>>>>
    CreateTenantHandlerV1(IMediator mediator
    , CreateTenantDto command
    , CancellationToken cancellationToken)
    {
        var result = await mediator.Send(
            new CreateTenantCommand(command), cancellationToken);

        return result.IsSuccess
            ? TypedResults.Created($"/api/tenants/{result.Data?.Id}",
                ApiResponse<TenantResponseDto>.Success(result.Data))
            : TypedResults.BadRequest(ApiResponse<TenantResponseDto>.Failure(result.Message!));
    }
}
