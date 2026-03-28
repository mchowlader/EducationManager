using Asp.Versioning;
using EduManager.Api.Extensions;
using EduManager.Application.DTOs.Feature.TeacherFeature;
using EduManager.Application.Features.Base.Commands;
using EduManager.Application.Features.TeacherFeature.Command;
using EduManager.Application.Features.TeacherFeature.Queries;
using EduManager.Domain.Common;
using EduManager.Domain.Constants;
using EduManager.Domain.Entities;
using MediatR;
using Microsoft.AspNetCore.Http.HttpResults;

namespace EduManager.Api.Endpoints;

public class TeacherEndpoints : IEndpoints
{
    public static void MapEndpoints(WebApplication app)
    {
        var versionSet = app.NewApiVersionSet()
            .HasApiVersion(new ApiVersion(1, 0))
            .ReportApiVersions()
            .Build();

        var group = app.MapGroup("/api/v{version:apiVersion}/teachers")
            .WithApiVersionSet(versionSet)
            .WithTags("Teachers");

        group.MapGet("/", GetAllHandlerV1)
            .WithName("GetAllTeachers")
            .WithSummary("Get all teachers")
            .RequireAuthorization(Permissions.For("Teacher", Permissions.View))
            .MapToApiVersion(1, 0);

        group.MapGet("/{id:long}", GetByIdHandlerV1)
            .WithName("GetTeacherById")
            .WithSummary("Get teacher by id")
            .RequireAuthorization(Permissions.For("Teacher", Permissions.View))
            .MapToApiVersion(1, 0);

        group.MapPost("/", CreateHandlerV1)
            .WithName("CreateTeacher")
            .WithSummary("Create teacher")
            .RequireAuthorization(Permissions.For("Teacher", Permissions.Create))
            .WithValidation<CreateTeacherDto>()
            .MapToApiVersion(1, 0);

        group.MapPatch("/{id:long}", UpdateHandlerV1)
            .WithName("UpdateTeacher")
            .WithSummary("Update teacher")
            .RequireAuthorization(Permissions.For("Teacher", Permissions.Update))
            .WithValidation<UpdateTeacherDto>()
            .MapToApiVersion(1, 0);

        group.MapDelete("/{id:long}", DeleteHandlerV1)
            .WithName("DeleteTeacher")
            .WithSummary("Delete teacher")
            .RequireAuthorization(Permissions.For("Teacher", Permissions.Delete))
            .MapToApiVersion(1, 0);
    }

    private static async Task<
    Results<
        Ok<ApiResponse<IEnumerable<TeacherResponseDto>>>,
        UnprocessableEntity<ApiResponse<IEnumerable<TeacherResponseDto>>>>>
    GetAllHandlerV1(IMediator mediator,
    CancellationToken cancellationToken,
    int pageNumber = 1,
    int pageSize = 10)
    {
        var result = await mediator.Send(new GetAllTeachersQuery(pageNumber, pageSize), cancellationToken);

        return result.IsSuccess
            ? TypedResults.Ok(ApiResponse<IEnumerable<TeacherResponseDto>>.Success(result.Data, result.Message!))
            : TypedResults.UnprocessableEntity(ApiResponse<IEnumerable<TeacherResponseDto>>.Failure(result.Message!, result.ErrorCode));
    }

    private static async Task<
    Results<
        Ok<ApiResponse<TeacherResponseDto>>,
        NotFound<ApiResponse<TeacherResponseDto>>>>
    GetByIdHandlerV1(long id,
    IMediator mediator,
    CancellationToken cancellationToken)
    {
        var result = await mediator.Send(new GetTeacherByIdQuery(id), cancellationToken);

        return result.IsSuccess
            ? TypedResults.Ok(ApiResponse<TeacherResponseDto>.Success(result.Data, result.Message!))
            : TypedResults.NotFound(ApiResponse<TeacherResponseDto>.Failure(result.Message!, result.ErrorCode));
    }

    private static async Task<
    Results<
        Created<ApiResponse<TeacherResponseDto>>,
        BadRequest<ApiResponse<TeacherResponseDto>>>> 
    CreateHandlerV1(CreateTeacherDto dto,
    IMediator mediator,
    CancellationToken cancellationToken)
    {
        var result = await mediator.Send(new CreateTeacherCommand(dto), cancellationToken);

        return result.IsSuccess
            ? TypedResults.Created($"/api/teachers/{result.Data?.Id}",
                ApiResponse<TeacherResponseDto>.Success(result.Data, result.Message!))
            : TypedResults.BadRequest(ApiResponse<TeacherResponseDto>.Failure(result.Message!, result.ErrorCode));
    }

    private static async Task<
    Results<
        Ok<ApiResponse<TeacherResponseDto>>,
        BadRequest<ApiResponse<TeacherResponseDto>>>> 
    UpdateHandlerV1(long id,
    UpdateTeacherDto dto,
    IMediator mediator,
    CancellationToken cancellationToken)
    {
        var result = await mediator.Send(new UpdateTeacherCommand(id, dto), cancellationToken);

        return result.IsSuccess
            ? TypedResults.Ok(ApiResponse<TeacherResponseDto>.Success(result.Data, result.Message!))
            : TypedResults.BadRequest(ApiResponse<TeacherResponseDto>.Failure(result.Message!, result.ErrorCode));
    }

    private static async Task<
    Results<
        NoContent,
        BadRequest<ApiResponse<object>>>> 
    DeleteHandlerV1(long id,
    IMediator mediator,
    CancellationToken cancellationToken)
    {
        var result = await mediator.Send(new DeleteTeacherCommand(id), cancellationToken);

        return result.IsSuccess
            ? TypedResults.NoContent()
            : TypedResults.BadRequest(ApiResponse<object>.Failure(result.Message!, result.ErrorCode));
    }
}
