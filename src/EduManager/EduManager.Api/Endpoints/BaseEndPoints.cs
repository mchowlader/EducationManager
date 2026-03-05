using EduManager.Application.Features.Base.Commands;
using EduManager.Application.Features.Base.Queries;
using EduManager.Domain.Common;
using EduManager.Domain.Constants;
using MediatR;
using Microsoft.AspNetCore.Mvc;

namespace EduManager.Api.Endpoints;

public abstract class BaseEndPoints<TEntity, TCreateDto, TUpdateDto, TResponseDto>
    where TEntity : BaseEntity, new()
    where TCreateDto : class
    where TUpdateDto : class
    where TResponseDto : class
{
    protected static string EntityName => typeof(TEntity).Name;
    protected static string Route => ToKebabCase(typeof(TEntity).Name);

    private static string ViewPermission => Permissions.For(EntityName, Permissions.View);
    private static string CreatePermission => Permissions.For(EntityName, Permissions.Create);
    private static string UpdatePermission => Permissions.For(EntityName, Permissions.Update);
    private static string DeletePermission => Permissions.For(EntityName, Permissions.Delete);

    protected static RouteGroupBuilder MapBaseEndPoints(IEndpointRouteBuilder app)
    {
        var group = app.MapGroup($"/api/{Route}")
            .WithTags(EntityName);

        group.MapGet("/", GetAllHandler)
            .WithName($"GetAll{EntityName}");

        group.MapGet("/{id:long}", GetByIdHandler)
            .WithName($"Get{EntityName}ById")
            .RequireAuthorization(ViewPermission);

        group.MapPost("/", CreateHandler)
            .WithName($"Create{EntityName}");
            //.RequireAuthorization(CreatePermission);

        group.MapPut("/{id:long}", UpdateHandler)
            .WithName($"Update{EntityName}")
            .RequireAuthorization(UpdatePermission);

        group.MapDelete("/{id:long}", DeleteHandler)
            .WithName($"Delete{EntityName}")
            .RequireAuthorization(DeletePermission);

        return group;
    }

    private static async Task<IResult> GetAllHandler(
        IMediator mediator, 
        [FromQuery] int pageNumber = 1,
        [FromQuery] int pageSize = 10)
    {
        var result = await mediator.Send(
            new BaseGetAllQuery<TEntity, TResponseDto>(pageNumber, pageSize));

        return result.IsSuccess
            ? Results.Ok(ApiResponse<IEnumerable<TResponseDto>>.Success(result.Data))
            : Results.BadRequest(ApiResponse<TResponseDto>.Failure(result.Message!));
    }


    private static async Task<IResult> GetByIdHandler(long id, IMediator mediator)
    {
        var result = await mediator.Send(
            new BaseGetByIdQuery<TEntity, TResponseDto>(id));

        return result.IsSuccess
            ? Results.Ok(ApiResponse<TResponseDto>.Success(result.Data))
            : Results.NotFound(ApiResponse<TResponseDto>.Failure(result.Message!));
    }

    private static async Task<IResult> CreateHandler(TCreateDto dto, IMediator mediator)
    {
        var result = await mediator.Send(
            new BaseCreateCommand<TEntity, TCreateDto, TResponseDto>(dto));

        return result.IsSuccess
            ? Results.Created($"/api/{Route}/{result.Data}",
                ApiResponse<TResponseDto>.Success(result.Data))
            : Results.BadRequest(ApiResponse<TResponseDto>.Failure(result.Message!));
    }

    private static async Task<IResult> UpdateHandler(
        long id, TUpdateDto dto, IMediator mediator)
    {
        var result = await mediator.Send(
            new BaseUpdateCommand<TEntity, TUpdateDto, TResponseDto>(id, dto));

        return result.IsSuccess
            ? Results.Ok(ApiResponse<TResponseDto>.Success(result.Data))
            : Results.BadRequest(ApiResponse<TResponseDto>.Failure(result.Message!));
    }

    private static async Task<IResult> DeleteHandler(long id, IMediator mediator)
    {
        var result = await mediator.Send(new BaseDeleteCommand<TEntity>(id));

        return result.IsSuccess
            ? Results.NoContent()
            : Results.BadRequest(ApiResponse<object>.Failure(result.Message!));
    }

    private static string ToKebabCase(string value) =>
        string.Concat(value.Select((x, i) =>
        i > 0 && char.IsUpper(x) ? "-" + x : x.ToString())).ToLower();
}
