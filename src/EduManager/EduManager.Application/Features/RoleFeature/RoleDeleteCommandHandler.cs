using EduManager.Application.Features.Base.Commands;
using EduManager.Domain.Common;
using EduManager.Domain.Entities;
using EduManager.Domain.Interfaces;
using MediatR;

namespace EduManager.Application.Features.RoleFeature;

public class RoleDeleteCommandHandler(
    IRepository<Role> repository,
    IUnitOfWork unitOfWork)
    : IRequestHandler<BaseDeleteCommand<Role>, Result<bool>>
{
    private readonly IRepository<Role> _repository = repository;
    private readonly IUnitOfWork _unitOfWork = unitOfWork;
    public Task<Result<bool>> Handle(BaseDeleteCommand<Role> request, CancellationToken cancellationToken)
    {
        throw new NotImplementedException();
    }
}
