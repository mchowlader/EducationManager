using Asp.Versioning;
using EduManager.Application.DTOs.Feature.Auth;
using EduManager.Application.Features.Auth.Commands;
using EduManager.Domain.Attributes;
using EduManager.Domain.Common;
using MediatR;
using Microsoft.AspNetCore.Http.HttpResults;
using System.Security.Claims;

namespace EduManager.Api.Endpoints;

public class AuthEndpoints : IEndpoints
{
    public static void MapEndpoints(WebApplication app)
    {
        var versionSet = app.NewApiVersionSet()
            .HasApiVersion(new ApiVersion(1, 0))
            .ReportApiVersions()
            .Build();

        var group = app.MapGroup("/api/v{version:apiVersion}/auth")
            .WithApiVersionSet(versionSet)
            .WithTags("Auth")
            .WithMetadata(new MasterRouteAttribute());

        var tenantGroup = app.MapGroup("/api/v{version:apiVersion}/auth")
            .WithApiVersionSet(versionSet)
            .WithTags("Auth")
            .WithMetadata(new TenantRouteAttribute());

        #region Admin
        group.MapPost("/admin/login", AdminLoginHandlerV1)
            .WithName("AdminLogin")
            .WithSummary("Super Admin Login")
            .AllowAnonymous()
            .MapToApiVersion(1, 0);

        group.MapPost("/admin/refresh", AdminRefreshTokenHandlerV1)
            .WithName("AdminRefreshToken")
            .WithSummary("Refresh Super Admin Token")
            .AllowAnonymous()
            .MapToApiVersion(1, 0);

        group.MapPost("/admin/logout", AdminLogoutHandlerV1)
            .WithName("AdminLogout")
            .WithSummary("Super Admin Logout")
            .RequireAuthorization()
            .MapToApiVersion(1, 0);
        #endregion

        #region Tenant
        tenantGroup.MapPost("/login", TenantLoginHandlerV1)
            .WithName("TenantLogin")
            .WithSummary("Tenant User Login")
            .AllowAnonymous()
            .MapToApiVersion(1, 0);

        tenantGroup.MapPost("/refresh", TenantRefreshTokenHandlerV1)
            .WithName("TenantRefreshToken")
            .WithSummary("Tenant user refresh token")
            .AllowAnonymous()
            .MapToApiVersion(1, 0);

        tenantGroup.MapPost("/logout", TenantLogoutHandlerV1)
            .WithName("TenantLogout")
            .WithSummary("Tenant User Logout")
            .RequireAuthorization()
            .MapToApiVersion(1, 0);

        #endregion
    }
        #region Admin
        private static async Task<
        Results<
            Ok<ApiResponse<bool>>,
            UnprocessableEntity<ApiResponse<bool>>
            >>
        AdminLogoutHandlerV1(
        IMediator mediator,
        ClaimsPrincipal user,
        CancellationToken cancellationToken)
        {
            var adminId = long.Parse(user.FindFirstValue(ClaimTypes.NameIdentifier)!);

            var result = await mediator.Send(new AdminLogoutCommand(adminId), cancellationToken);

            return result.IsSuccess
                ? TypedResults.Ok(ApiResponse<bool>.Success(result.Data))
                : TypedResults.UnprocessableEntity(ApiResponse<bool>.Failure(result.Message!));
        }

        private static async Task<
        Results<
            Ok<ApiResponse<TokenResponseDto>>,
            UnprocessableEntity<ApiResponse<TokenResponseDto>>
            >>
        AdminLoginHandlerV1(
        AdminLoginDto dto,
        IMediator mediator,
        CancellationToken cancellationToken)
        {
            var result = await mediator.Send(
                new AdminLoginCommand(dto), cancellationToken);

            return result.IsSuccess
                ? TypedResults.Ok(ApiResponse<TokenResponseDto>.Success(result.Data))
                : TypedResults.UnprocessableEntity(ApiResponse<TokenResponseDto>.Failure(result.Message!));
        }

        private static async Task<
        Results<
            Ok<ApiResponse<TokenResponseDto>>,
            UnprocessableEntity<ApiResponse<TokenResponseDto>>
            >>
        AdminRefreshTokenHandlerV1(
        RefreshTokenDto dto,
        IMediator mediator,
        CancellationToken cancellationToken)
        {
            var result = await mediator.Send(
                new AdminRefreshTokenCommand(dto), cancellationToken);

            return result.IsSuccess
                ? TypedResults.Ok(ApiResponse<TokenResponseDto>.Success(result.Data))
                : TypedResults.UnprocessableEntity(ApiResponse<TokenResponseDto>.Failure(result.Message!));
        }
        #endregion

        #region Tenant
        private static async Task<
        Results<
            Ok<ApiResponse<TokenResponseDto>>,
            UnprocessableEntity<ApiResponse<TokenResponseDto>>
            >>
        TenantLoginHandlerV1(
        TenantLoginDto dto,
        IMediator mediator,
        CancellationToken cancellationToken)
        {
            var result = await mediator.Send(
                new TenantLoginCommand(dto), cancellationToken);

            return result.IsSuccess
                ? TypedResults.Ok(ApiResponse<TokenResponseDto>.Success(result.Data))
                : TypedResults.UnprocessableEntity(ApiResponse<TokenResponseDto>.Failure(result.Message!));
        }

        private static async Task<
        Results<
            Ok<ApiResponse<TokenResponseDto>>,
            UnprocessableEntity<ApiResponse<TokenResponseDto>>
            >>
        TenantRefreshTokenHandlerV1(
        RefreshTokenDto dto,
        IMediator mediator,
        CancellationToken cancellationToken)
        {
            var result = await mediator.Send(new TenantRefreshTokenCommand(dto), cancellationToken);

            return result.IsSuccess
                ? TypedResults.Ok(ApiResponse<TokenResponseDto>.Success(result.Data))
                : TypedResults.UnprocessableEntity(ApiResponse<TokenResponseDto>.Failure(result.Message!));
        }
        private static async Task<
        Results<
            Ok<ApiResponse<object>>,
            UnprocessableEntity<ApiResponse<object>>
            >>
        TenantLogoutHandlerV1(
        IMediator mediator,
        ClaimsPrincipal user,
        CancellationToken cancellationToken)
        {
            var userId = long.Parse(user.FindFirstValue(ClaimTypes.NameIdentifier)!);

            var result = await mediator.Send(
                new TenantLogoutCommand(userId), cancellationToken);

            return result.IsSuccess
                ? TypedResults.Ok(ApiResponse<object>.Success(null, result.Message!))
                : TypedResults.UnprocessableEntity(ApiResponse<object>.Failure(result.Message!));
        }
        #endregion
}