using EduManager.Application.DTOs.Feature.Shared;
using EduManager.Application.DTOs.Feature.TeacherFeature;
using EduManager.Application.Interfaces;
using EduManager.Domain.Common;
using EduManager.Domain.Entities;
using EduManager.Domain.Interfaces;
using EduManager.Domain.Interfaces.Repositories;
using EduManager.Infrastructure.Repositories;
using Microsoft.Extensions.Logging;

namespace EduManager.Infrastructure.Services;

public class TeacherService(
    ITeacherRepository teacherRepository,
    IUserRepository userRepository,
    IRepository<UserProfile> userProfileRepository,
    IRoleRepository roleRepository,
    IUserRoleRepository userRoleRepository,
    IEncryptionService encryption,
    IUnitOfWork unitOfWork,
    ILogger<Teacher> logger) : ITeacherService
{
    #region Create
    public async Task<Result<TeacherResponseDto>> CreateAsync(CreateTeacherDto dto, CancellationToken ct = default)
    {
        var emailExists = await teacherRepository.EmailExistsAsync(dto.Email, ct);

        if (emailExists)
            return Result<TeacherResponseDto>.Failure("Email already exists.");

        var teacherRole = await GetTeacherRoleAsync(ct);

        if (teacherRole is null)
            return Result<TeacherResponseDto>.Failure("Teacher role not found.");

        await using var transaction = await unitOfWork.BeginTransactionAsync(ct);

        try
        {
            var user = await CreateUserAsync(dto, ct);
            var profile = await CreateUserProfileAsync(dto, user.Id, ct);
            var teacher = await CreateTeacherAsync(dto, user.Id, ct);
            await AssignTeacherRoleAsync(user.Id, teacherRole.Id, ct);

            await unitOfWork.SaveChangesAsync(ct);
            await transaction.CommitAsync(ct);

            return Result<TeacherResponseDto>.Success(BuildResponse(teacher, user, profile), "Teacher created successfully.");
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "Failed to create teacher: {Message}", ex.Message);
            await transaction.RollbackAsync(ct);
            return Result<TeacherResponseDto>.Failure("Failed to create teacher.");
        }
    }
    private async Task<User> CreateUserAsync(CreateTeacherDto dto, CancellationToken ct)
    {
        try
        {
            var user = new User
            {
                Email = dto.Email,
                PasswordHash = encryption.HashPassword(dto.Password),
                IsActive = true
            };

            await userRepository.AddAsync(user, ct);
            await unitOfWork.SaveChangesAsync(ct);
            return user;
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "Failed to create teacher: {Message}", ex.Message);
            throw;
        }


    }
    private async Task<UserProfile> CreateUserProfileAsync(CreateTeacherDto dto, long userId, CancellationToken ct)
    {
        var profile = new UserProfile
        {
            FullName = dto.FullName,
            Mobile = dto.Mobile,
            DateOfBirth = dto.DateOfBirth,
            UserId = userId,
            Address = new Address
            {
                Division = dto.Address.Division,
                District = dto.Address.District,
                Thana = dto.Address.Thana,
                City = dto.Address.City,
                PostalCode = dto.Address.PostalCode
            }
        };


        await userProfileRepository.AddAsync(profile, ct);
        return profile;
    }
    private async Task<Teacher> CreateTeacherAsync(CreateTeacherDto dto, long userId, CancellationToken ct)
    {
        var teacher = new Teacher
        {
            Designation = dto.Designation,
            IsActive = true,
            UserId = userId
        };
        await teacherRepository.AddAsync(teacher, ct);
        return teacher;
    }
    private async Task AssignTeacherRoleAsync(long userId, long roleId, CancellationToken ct)
    {
        var userRole = new UserRole
        {
            UserId = userId,
            RoleId = roleId
        };
        await userRoleRepository.AddAsync(userRole, ct);
    }
    private async Task<Role?> GetTeacherRoleAsync(CancellationToken ct)
        => await roleRepository.GetByNameAsync("Teacher", ct);
    private static TeacherResponseDto BuildResponse(Teacher teacher, User user, UserProfile profile)
        => new(
            teacher.Id,
            teacher.TeacherCode,
            teacher.Designation,
            teacher.IsActive,
            user.UserCode,
            user.Email,
            profile.FullName,
            profile.Mobile,
            profile.DateOfBirth,
            new AddressDto(
                profile.Address.Division,
                profile.Address.District,
                profile.Address.Thana,
                profile.Address.City,
                profile.Address.PostalCode)
        );
    #endregion

    #region Update
    public async Task<Result<TeacherResponseDto>> UpdateAsync(long id, UpdateTeacherDto dto, CancellationToken ct = default)
    {
        var teacher = await teacherRepository.GetByIdWithDetailsAsync(id, ct);

        if (teacher is null)
            return Result<TeacherResponseDto>.Failure("Teacher not found");

        await using var transaction = await unitOfWork.BeginTransactionAsync(ct);

        try
        {
            if (dto.Designation is not null)
                teacher.Designation = dto.Designation;

            UpdateProfileAsync(teacher.User.Profile, dto, ct);

            if (dto.Address is not null)
                UpdateAddressAsync(teacher.User.Profile.Address, dto, ct);

            teacherRepository.Update(teacher);
            await unitOfWork.SaveChangesAsync(ct);
            await transaction.CommitAsync(ct);

            return Result<TeacherResponseDto>.Success(
                BuildResponse(teacher, teacher.User, teacher.User.Profile),
                "Teacher updated successfully.");
        }
        catch (Exception ex)
        {
            logger.LogError(ex.StackTrace);
            await transaction.RollbackAsync(ct);
            return Result<TeacherResponseDto>.Failure("Failed to update teacher.");
        }
    }
    private static void UpdateProfileAsync(UserProfile profile, UpdateTeacherDto dto, CancellationToken ct = default)
    {
        if (dto.FullName is not null)
            profile.FullName = dto.FullName;

        if (dto.Mobile is not null)
            profile.Mobile = dto.Mobile;

        if (dto.DateOfBirth is not null)
            profile.DateOfBirth = dto.DateOfBirth;
    }
    private static void UpdateAddressAsync(Address address, UpdateTeacherDto dto, CancellationToken ct = default)
    {
        if (dto.Address is null)
            return;

        if (dto.Address.Division is not null)
            address.Division = dto.Address.Division;

        if (dto.Address.District is not null)
            address.District = dto.Address.District;

        if (dto.Address.Thana is not null)
            address.Thana = dto.Address.Thana;

        if (dto.Address.City is not null)
            address.City = dto.Address.City;

        if (dto.Address.PostalCode is not null)
            address.PostalCode = dto.Address.PostalCode;
    }
    #endregion

    #region Delete
    public async Task<Result<bool>> DeleteAsync(long id, CancellationToken ct = default)
    {
        var teacher = await teacherRepository.GetByIdWithDetailsAsync(id, ct);

        if (teacher is null)
            return Result<bool>.Failure("Teacher not found.");

        await using var transaction = await unitOfWork.BeginTransactionAsync(ct);

        try
        {
            DeactivateTeacher(teacher);
            DeactivateUser(teacher.User);  
            DeactivateUserProfile(teacher.User.Profile); 
            await DeactivateUserRolesAsync(teacher.UserId, ct);

            await unitOfWork.SaveChangesAsync(ct);
            await transaction.CommitAsync(ct);

            return Result<bool>.Success(true, "Teacher deleted successfully.");
        }
        catch
        {
            await transaction.RollbackAsync(ct);
            return Result<bool>.Failure("Failed to delete teacher.");
        }
    }
    private void DeactivateTeacher(Teacher teacher)
    {
        teacher.IsDelete = true;
        teacher.IsActive = false;
        teacherRepository.Update(teacher);
    }
    private void DeactivateUser(User user)
    {
        user.IsActive = false;
        user.RefreshToken = null;
        user.RefreshTokenExpiry = null;
        userRepository.Update(user);
    }
    private void DeactivateUserProfile(UserProfile profile)
    {
        profile.IsDelete = true;
        userProfileRepository.Update(profile);
    }
    private async Task DeactivateUserRolesAsync(long userId, CancellationToken ct)
    {
        var userRoles = await userRoleRepository.GetByUserIdAsync(userId, ct);
        foreach (var userRole in userRoles)
        {
            userRole.IsDelete = true;
            userRoleRepository.Update(userRole);
        }
    }
    #endregion
}
