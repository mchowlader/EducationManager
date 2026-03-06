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

        group.MapPut("/", UpdateTenantHandlerV1)
            .WithName("UpdateTenantV1")
            .WithSummary("Update Tenant")
            .WithDescription("Update an exiting tenant and returns the updated resource")
            .MapToApiVersion(1, 0);

        group.MapGet("/", GetAllTenantHandlerV1)
           .WithName("GetAllTenantV1")
           .WithSummary("Gat All Tenants")
           .WithDescription("Returns a paginated list of all tenants. Use pageNumber and pageSize to control pagination.")
           .MapToApiVersion(1, 0);
    }

    private static async Task<Results<Ok<ApiResponse<IEnumerable<TenantResponseDto>>>,
                                    BadRequest<ApiResponse<TenantResponseDto>>>> GetAllTenantHandlerV1(
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
            BadRequest<ApiResponse<TenantResponseDto>>
            >> 
        UpdateTenantHandlerV1(IMediator mediator
        , UpdateTenantDto command
        , CancellationToken cancellationToken )
    {
        var result = await mediator.Send(
            new UpdateTenantCommand(command), cancellationToken);

        return result.IsSuccess
            ? TypedResults.Ok(ApiResponse<TenantResponseDto>.Success(result.Data))
            : TypedResults.BadRequest(ApiResponse<TenantResponseDto>.Failure(result.Message!));
    }

    private static async Task<
        Results<
            Created<ApiResponse<TenantResponseDto>>, 
            BadRequest<ApiResponse<TenantResponseDto>>
            >>
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
