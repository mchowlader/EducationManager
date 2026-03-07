using AutoMapper;
using EduManager.Application.DTOs.Feature.TenantFeature;
using EduManager.Domain.Common;
using EduManager.Domain.Interfaces;
using EduManager.Domain.Interfaces.Repositories;
using Hangfire;
using MediatR;

namespace EduManager.Application.Features.TenantFeature.Commands;

public class UpdateTenantCommandHandler(
          ITenantRepository repository
        , IMasterUnitOfWork unitOfWork
        , IMapper mapper)
    : IRequestHandler<UpdateTenantCommand, Result<TenantResponseDto>>
{
    private readonly ITenantRepository _repository = repository;
    private readonly IMasterUnitOfWork _unitOfWork = unitOfWork;
    private readonly IMapper _mapper = mapper;

    public async Task<Result<TenantResponseDto>> Handle(
        UpdateTenantCommand request, CancellationToken cancellationToken)
    {
        var tenant = await _repository.GetByIdAsync(request.id);

        if (tenant is null)
            return Result<TenantResponseDto>.Failure("Tenant not found.");

        if (request.Dto.Email is not null && tenant.Email != request.Dto.Email)
        {
            var emailExists = await _repository.EmailExistsAsync(request.Dto.Email, cancellationToken);
            if (emailExists)
                return Result<TenantResponseDto>.Failure("Email already exists.");

            tenant.Email = request.Dto.Email;
        }

        if (request.Dto.Name is not null)
            tenant.Name = request.Dto.Name;

        if (request.Dto.Mobile is not null)
            tenant.Mobile = request.Dto.Mobile;

        _repository.Update(tenant);
        await _unitOfWork.SaveChangesAsync(cancellationToken);

        return Result<TenantResponseDto>.Success(
            _mapper.Map<TenantResponseDto>(tenant), "Tenant updated successfully");
    }
}
