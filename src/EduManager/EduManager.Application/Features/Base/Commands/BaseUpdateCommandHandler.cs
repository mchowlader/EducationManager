using AutoMapper;
using EduManager.Domain.Common;
using EduManager.Domain.Interfaces;
using MediatR;

namespace EduManager.Application.Features.Base.Commands;

public class BaseUpdateCommandHandler<TEntity, TUpdateDto, TResponseDto>(
    IRepository<TEntity> repository,
    IUnitOfWork unitOfWork,
    IMapper mapper)
    : IRequestHandler<BaseUpdateCommand<TEntity, TUpdateDto, TResponseDto>, Result<TResponseDto>>
    where TEntity : BaseEntity, new()
    where TUpdateDto : class
    where TResponseDto : class
{
    public async Task<Result<TResponseDto>> Handle(BaseUpdateCommand<TEntity, TUpdateDto, TResponseDto> request, CancellationToken cancellationToken)
    {
        try
        {
            var entity = await repository.GetByIdAsync(request.Id, cancellationToken);

            if (entity is null)
                return Result<TResponseDto>.Failure($"{typeof(TEntity).Name} not found");

            mapper.Map(request.Dto, entity);

            repository.Update(entity);
            await unitOfWork.SaveChangesAsync(cancellationToken);

            var data = mapper.Map<TResponseDto>(entity);

            return Result<TResponseDto>.Success(data, $"{typeof(TEntity).Name} updated successfully");
        }
        catch (Exception)
        {
            return Result<TResponseDto>.Failure($"Internal Server Error");
        }
    }
}
