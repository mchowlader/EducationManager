using EduManager.Domain.Common;
using EduManager.Infrastructure.Persistence;

namespace EduManager.Infrastructure.Repositories;

public class EduRepository<T>(EduDbContext context)
    : BaseRepository<T, EduDbContext>(context)
    where T : BaseEntity, new()
{
}