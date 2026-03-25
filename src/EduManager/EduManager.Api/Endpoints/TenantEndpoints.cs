using Asp.Versioning;
using EduManager.Application.DTOs.Feature.TenantFeature;
using EduManager.Application.Features.TenantFeature.Commands;
using EduManager.Application.Features.TenantFeature.Queries;
using EduManager.Domain.Attributes;
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

        var group = app.MapGroup("/api/v{version:apiVersion}/tenants")
            .WithApiVersionSet(versionSet)
            .WithTags("Tenants")
            .WithMetadata(new MasterRouteAttribute())
            .RequireAuthorization();

        group.MapPost("/", CreateTenantHandlerV1)
            .RequireAuthorization("MasterAdminAccess")  // Owner + SuperAdmin
            .WithName("CreateTenantV1")
            .WithSummary("Create Tenant")
            .WithDescription("Creates a new tenant and returns the created resource")
            .MapToApiVersion(1, 0);

        group.MapPatch("/{id:long}", UpdateTenantHandlerV1)
            .RequireAuthorization("MasterAdminAccess")  // Owner + SuperAdmin
            .WithName("UpdateTenantV1")
            .WithSummary("Update Tenant")
            .WithDescription("Update an existing tenant and returns the updated resource")
            .MapToApiVersion(1, 0);

        group.MapGet("/", GetAllTenantHandlerV1)
            .RequireAuthorization("MasterViewAccess")   // Owner + SuperAdmin + Support
            .RequireRateLimiting("strict")
            .WithName("GetAllTenantV1")
            .WithSummary("Get All Tenants")
            .WithDescription("Returns a paginated list of all tenants.")
            .MapToApiVersion(1, 0);

        group.MapGet("/{id:long}", GetByIdTenantHandlerV1)
            .RequireAuthorization("MasterViewAccess")   // Owner + SuperAdmin + Support
            .WithName("GetByIdTenantV1")
            .WithSummary("Get a Tenant")
            .WithDescription("Returns a Tenant record")
            .MapToApiVersion(1, 0);

        group.MapDelete("/{id:long}", DeleteHandlerV1)
            .RequireAuthorization("MasterOwnerOnly")    // Only Owner
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
            : TypedResults.BadRequest(ApiResponse<object>.Failure(result.Message!, result.ErrorCode));
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
            : TypedResults.BadRequest(ApiResponse<TenantResponseDto>.Failure(result.Message!, result.ErrorCode));
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
            : TypedResults.BadRequest(ApiResponse<TenantResponseDto>.Failure(result.Message!, result.ErrorCode));
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
            : TypedResults.BadRequest(ApiResponse<TenantResponseDto>.Failure(result.Message!, result.ErrorCode));
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
            : TypedResults.BadRequest(ApiResponse<TenantResponseDto>.Failure(result.Message!, result.ErrorCode));
    }
}
