using AutoMapper;
using EduManager.Domain.Common;
using EduManager.Domain.Interfaces;
using MediatR;

namespace EduManager.Application.Features.Base.Commands;

public class BaseCreateCommandHandler<TEntity, TCreateDto, TResponseDto>(
    IRepository<TEntity> repository,
    IUnitOfWork unitOfWork,
    IMapper mapper)
    : IRequestHandler<BaseCreateCommand<TEntity, TCreateDto, TResponseDto>, Result<TResponseDto>>
    where TEntity : BaseEntity, new()
    where TCreateDto : class
    where TResponseDto : class
{
    public async Task<Result<TResponseDto>> Handle(BaseCreateCommand<TEntity, TCreateDto, TResponseDto> request, CancellationToken cancellationToken)
    {
        try
        {
            var entity = mapper.Map<TEntity>(request.Dto);

            await repository.AddAsync(entity, cancellationToken);
            var result = await unitOfWork.SaveChangesAsync(cancellationToken);

            if (result <= 0)
                return Result<TResponseDto>.Failure($"{typeof(TEntity).Name} Failed to create");

            var data = mapper.Map<TResponseDto>(entity);

            return Result<TResponseDto>.Success(data, $"{typeof(TEntity).Name}  Created successfully");
        }
        catch (Exception)
        {
            return Result<TResponseDto>.Failure($"{typeof(TEntity).Name} Failed to create. Internal Serval Error.");
        }
    }
}
