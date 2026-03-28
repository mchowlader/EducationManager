using EduManager.Application.DTOs.Feature.Shared;
using EduManager.Application.DTOs.Feature.TeacherFeature;
using EduManager.Domain.Common;
using EduManager.Domain.Interfaces.Repositories;
using MediatR;

namespace EduManager.Application.Features.TeacherFeature.Queries;

public class GetAllTeachersQueryHandler(ITeacherRepository repository)
    : IRequestHandler<GetAllTeachersQuery, Result<IEnumerable<TeacherResponseDto>>>
{
    public async Task<Result<IEnumerable<TeacherResponseDto>>> Handle(GetAllTeachersQuery request, CancellationToken cancellationToken)
    {
        var teacher = await repository.GetAllWithDetailsAsync(request.PageNumber, request.PageSize, cancellationToken);

        if(!teacher.Any())
            return Result<IEnumerable<TeacherResponseDto>>.Success(null, "No data found.");

        var response = teacher
        .Select(t => 
            new TeacherResponseDto
            (
                t.Id,
                t.TeacherCode,
                t.Designation,
                t.IsActive,
                t.User.UserCode,
                t.User.Email,
                t.User.Profile.FullName,
                t.User.Profile.Mobile,
                t.User.Profile.DateOfBirth,
                new AddressDto
                (
                    t.User.Profile.Address.Division,
                    t.User.Profile.Address.District,
                    t.User.Profile.Address.Thana,
                    t.User.Profile.Address.City,
                    t.User.Profile.Address.PostalCode
                )
            )
        );

        return Result<IEnumerable<TeacherResponseDto>>.Success(response, "Teachers retrive successfully.");
    }
}
