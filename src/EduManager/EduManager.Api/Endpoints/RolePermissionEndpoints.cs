using Asp.Versioning;
using EduManager.Application.DTOs.Feature.RoleFeature;
using EduManager.Application.Features.RoleFeature;
using EduManager.Domain.Common;
using EduManager.Domain.Entities;
using MediatR;
using Microsoft.AspNetCore.Components.Forms;
using Microsoft.AspNetCore.Http.HttpResults;

namespace EduManager.Api.Endpoints;

public class RolePermissionEndpoints : IEndpoints
{
    public static void MapEndpoints(WebApplication app)
    {
        var versionSet = app.NewApiVersionSet()
            .HasApiVersion(new ApiVersion(1, 0))
            .ReportApiVersions()
            .Build();

        var group = app.MapGroup("/api/v{version:apiVersion}/roles")
            .WithApiVersionSet(versionSet)
            .WithTags("RolePermissions");

        group.MapPost("/{roleId:long}/permissions/", AddPermissionHandlerV1)
            .WithName("AddPermission")
            .WithSummary("Add permission to role")
            .RequireAuthorization("RolePermission.Create")
            .MapToApiVersion(1, 0);

        group.MapDelete("/{roleId:long}/permissions/{permission}", RemovePermissionHandlerV1)
            .WithName("RemovePermission")
            .WithSummary("Remove permission from role")
            .RequireAuthorization("Permission.Delete")
            .HasApiVersion(1, 0);

        group.MapPost("/{userId:long}/roles", AssignUserRoleHandlerV1)
            .WithName("AssignUserRole")
            .WithSummary("Assign role to user")
            .RequireAuthorization("UserRole.Create")
            .MapToApiVersion(1, 0);

        group.MapDelete("/{userId:long}/roles/{roleId:long}", RemoveUserRoleHandlerV1)
            .WithName("RemoveUserRole")
            .WithSummary("Remove role from user")
            .RequireAuthorization("UserRole.Delete")
            .MapToApiVersion(1, 0);
    }

    private static async Task<
    Results
        <Ok<ApiResponse<UserRoleDto>>,
        UnprocessableEntity<ApiResponse<UserRoleDto>>>>
    AssignUserRoleHandlerV1(long userId
    , AssignUserRoleDto dto
    , IMediator mediator
    , CancellationToken cancellationToken)
    {
        var result = await mediator.Send(
            new AssignUserRoleCommand(userId, dto), cancellationToken);

        return result.IsSuccess
            ? TypedResults.Ok(ApiResponse<UserRoleDto>.Success(result.Data, result.Message!))
            : TypedResults.UnprocessableEntity(ApiResponse<UserRoleDto>.Failure(result.Message!, result.ErrorCode));
    }

    private static async Task<
    Results
        <Created<ApiResponse<object>>,
        UnprocessableEntity<ApiResponse<object>>>> 
    AddPermissionHandlerV1(long roleId
    , AddRolePermissionDto dto
    , IMediator mediator
    , CancellationToken cancellationToken)
    {
        var result = await mediator.Send(new AddRolePermissionCommand(roleId, dto), cancellationToken);

        return result.IsSuccess
            ? TypedResults.Created( string.Empty,
                ApiResponse<object>.Success(null, result.Message!))
            : TypedResults.UnprocessableEntity(
                ApiResponse<object>.Failure(result.Message!, result.ErrorCode));
    }

    private static async Task<
    Results
        <Ok<ApiResponse<object>>,
        UnprocessableEntity<ApiResponse<object>>>> 
    RemovePermissionHandlerV1(long roleId
    , string permission
    , IMediator mediator
    , CancellationToken cancellationToken)
    {
        var result = await mediator.Send(new RemoveRolePermissionCommand(roleId, permission), cancellationToken);

        return result.IsSuccess
            ? TypedResults.Ok(ApiResponse<object>.Success(result.Data))
            : TypedResults.UnprocessableEntity(ApiResponse<object>.Failure(result.Message!, result.ErrorCode));
    }

    private static async Task<
    Results
        <Ok<ApiResponse<object>>,
        UnprocessableEntity<ApiResponse<object>>>>
    RemoveUserRoleHandlerV1(long userId
    , long roleId
    , IMediator mediator
    , CancellationToken cancellationToken)
    {
        var result = await mediator.Send(
            new RemoveUserRoleCommand(userId, roleId), cancellationToken);

        return result.IsSuccess
            ? TypedResults.Ok(ApiResponse<object>.Success(result.Message!))
            : TypedResults.UnprocessableEntity(ApiResponse<object>.Failure(result.Message!, result.ErrorCode));
    }
}
