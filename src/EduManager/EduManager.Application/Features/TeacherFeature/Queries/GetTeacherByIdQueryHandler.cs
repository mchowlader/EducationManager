using EduManager.Application.DTOs.Feature.Shared;
using EduManager.Application.DTOs.Feature.TeacherFeature;
using EduManager.Domain.Common;
using EduManager.Domain.Interfaces.Repositories;
using MediatR;

namespace EduManager.Application.Features.TeacherFeature.Queries;

public class GetTeacherByIdQueryHandler(ITeacherRepository repository)
    : IRequestHandler<GetTeacherByIdQuery, Result<TeacherResponseDto>>
{
    public async Task<Result<TeacherResponseDto>> Handle(GetTeacherByIdQuery request, CancellationToken cancellationToken)
    {
        var teacher = await repository.GetByIdWithDetailsAsync(request.Id, cancellationToken);

        if (teacher is null)
            return Result<TeacherResponseDto>.Failure("Data not found.");

        var response = new TeacherResponseDto(
            teacher.Id,
            teacher.TeacherCode,
            teacher.Designation,
            teacher.IsActive,
            teacher.User.UserCode,
            teacher.User.Email,
            teacher.User.Profile.FullName,
            teacher.User.Profile.Mobile,
            teacher.User.Profile.DateOfBirth,
            new AddressDto(
                teacher.User.Profile.Address.Division,
                teacher.User.Profile.Address.District,
                teacher.User.Profile.Address.Thana,
                teacher.User.Profile.Address.City,
                teacher.User.Profile.Address.PostalCode)
            );

        return Result<TeacherResponseDto>.Success(response, "Teacher retrive successfully.");
    }
}
