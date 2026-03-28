using EduManager.Domain.Entities;

namespace EduManager.Domain.Interfaces.Repositories;

public interface ITeacherRepository : IRepository<Teacher>
{
    Task<Teacher?> GetByIdWithDetailsAsync(long id, CancellationToken ct = default);
    Task<IEnumerable<Teacher>> GetAllWithDetailsAsync(int pageNumber, int pageSize, CancellationToken ct = default);
    Task<bool> EmailExistsAsync(string email, CancellationToken ct = default);
}