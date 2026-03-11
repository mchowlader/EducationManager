using AutoMapper;
using EduManager.Application.DTOs.Feature.TenantFeature;
using EduManager.Application.Interfaces;
using EduManager.Domain.Common;
using EduManager.Domain.Entities.Master;
using EduManager.Domain.Enums;
using EduManager.Domain.Interfaces;
using EduManager.Domain.Interfaces.Repositories;
using Hangfire;
using MediatR;
using System.Security.Cryptography;

namespace EduManager.Application.Features.TenantFeature.Commands;

public class CreateTenantCommandHandler(
        ITenantRepository repository
        , IMasterUnitOfWork unitOfWork
        , IBackgroundJobClient backgroundJob
        , IMapper mapper)
    : IRequestHandler<CreateTenantCommand, Result<TenantResponseDto>>
{
    private readonly ITenantRepository _repository = repository;
    private readonly IMasterUnitOfWork _unitOfWork = unitOfWork;
    private readonly IBackgroundJobClient _backgroundJob = backgroundJob;
    private readonly IMapper _mapper = mapper;

    public async Task<Result<TenantResponseDto>> Handle(CreateTenantCommand request, CancellationToken cancellationToken)
    {
        var slugExits = await _repository.SlugExistsAsync(request.Dto.Slug, cancellationToken);

        if (slugExits)
            return Result<TenantResponseDto>.Failure("Slug already exits.");

        var emailExits = await _repository.EmailExistsAsync(request.Dto.Email, cancellationToken);

        if (emailExits)
            return Result<TenantResponseDto>.Failure("Email already exits.");

        var tenant = _mapper.Map<Tenant>(request.Dto);
        tenant.Status = TenantStatus.Pending;
        tenant.EncryptionSalt = Convert.ToBase64String(RandomNumberGenerator.GetBytes(16));

        await _repository.AddAsync(tenant, cancellationToken);
        await _unitOfWork.SaveChangesAsync(cancellationToken);

        _backgroundJob.Enqueue<ITenantCreationJob>(job => job.ExecutionAsync(tenant.Id));

        return Result<TenantResponseDto>.Success(
            _mapper.Map<TenantResponseDto>(tenant), "Tenant creation initiated");
    }
}
