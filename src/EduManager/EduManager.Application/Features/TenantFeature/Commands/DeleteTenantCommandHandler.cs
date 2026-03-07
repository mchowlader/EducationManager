using EduManager.Application.Interfaces;
using EduManager.Domain.Common;
using EduManager.Domain.Enums;
using EduManager.Domain.Interfaces;
using EduManager.Domain.Interfaces.Repositories;
using Hangfire;
using MediatR;

namespace EduManager.Application.Features.TenantFeature.Commands;

public class DeleteTenantCommandHandler(
        ITenantRepository repository
        , IMasterUnitOfWork unitOfWork
    , IBackgroundJobClient backgroundJob)
    : IRequestHandler<DeleteTenantCommand, Result<ApiResponse<object>>>
{
    private readonly ITenantRepository _repository = repository;
    private readonly IMasterUnitOfWork _unitOfWork = unitOfWork;
    private readonly IBackgroundJobClient _backgroundJob = backgroundJob;

    public async Task<Result<ApiResponse<object>>> Handle(DeleteTenantCommand request, CancellationToken cancellationToken)
    {
        var tenant = await _repository.GetByIdAsync(request.id);
        if (tenant is null)
            return Result<ApiResponse<object>>.Success(null, "No data found");

        tenant.IsDelete = true;
        tenant.Status = TenantStatus.PendingDeletion;

        _repository.Update(tenant);
        await _unitOfWork.SaveChangesAsync();

        _backgroundJob.Schedule<ITenantDeletionJob>(job => job.ExecuteAsync(tenant.Id), TimeSpan.FromDays(120));
        //need to send email user that after 120 days you information will be remove permanently

        return Result<ApiResponse<object>>.Success(
            new ApiResponse<object>(), "Tenant deletion scheduled");
    }
}
