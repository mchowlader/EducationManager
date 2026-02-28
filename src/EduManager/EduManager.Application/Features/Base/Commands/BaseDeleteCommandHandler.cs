using EduManager.Domain.Common;
using EduManager.Domain.Interfaces;
using MediatR;

namespace EduManager.Application.Features.Base.Commands;

public class BaseDeleteCommandHandler<TEntity>(
    IRepository<TEntity> repository,
    IUnitOfWork unitOfWork)
    : IRequestHandler<BaseDeleteCommand<TEntity>, Result<bool>>
    where TEntity : BaseEntity, new()
{
    public async Task<Result<bool>> Handle(BaseDeleteCommand<TEntity> request, CancellationToken cancellationToken)
    {
        try
        {
            var entity = await repository.GetByIdAsync(request.Id, cancellationToken);

            if (entity is null)
                return Result<bool>.Failure($"{typeof(TEntity).Name} not found");

            entity.IsDelete = true;
            repository.Update(entity);
            await unitOfWork.SaveChangesAsync(cancellationToken);

            return Result<bool>.Success(true, $"{typeof(TEntity).Name} deleted successfully");
        }
        catch (Exception)
        {
            return Result<bool>.Failure($"Internal Server Error");
        }
    }
}
