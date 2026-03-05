using AutoMapper;
using EduManager.Domain.Common;
using EduManager.Domain.Interfaces;
using MediatR;

namespace EduManager.Application.Features.Base.Commands;

public class BaseCreateCommandHandler<TEntity, TCreateDto, TResponseDto>
    : IRequestHandler<BaseCreateCommand<TEntity, TCreateDto, TResponseDto>, Result<TResponseDto>>
    where TEntity : BaseEntity, new()
    where TCreateDto : class
    where TResponseDto : class
{
    private readonly IRepository<TEntity> _repository;
    private readonly IUnitOfWork _unitOfWork;
    private readonly IMapper _mapper;

    public BaseCreateCommandHandler(IRepository<TEntity> repository, IUnitOfWork unitOfWork, IMapper mapper)
    {
        _repository = repository;
        _unitOfWork = unitOfWork;
        _mapper = mapper;
    }


    public async Task<Result<TResponseDto>> Handle(BaseCreateCommand<TEntity, TCreateDto, TResponseDto> request, CancellationToken cancellationToken)
    {
        try
        {
            var entity = _mapper.Map<TEntity>(request.Dto);

            await _repository.AddAsync(entity, cancellationToken);
            var result = await _unitOfWork.SaveChangesAsync(cancellationToken);

            if (result <= 0)
                return Result<TResponseDto>.Failure($"{typeof(TEntity).Name} Failed to create");

            var data = _mapper.Map<TResponseDto>(entity);

            return Result<TResponseDto>.Success(data, $"{typeof(TEntity).Name}  Created successfully");
        }
        catch (Exception)
        {
            return Result<TResponseDto>.Failure($"{typeof(TEntity).Name} Failed to create. Internal Serval Error.");
        }
    }
}
