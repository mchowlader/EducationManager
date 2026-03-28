using EduManager.Domain.Entities;
using EduManager.Domain.Interfaces.Repositories;
using EduManager.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;

namespace EduManager.Infrastructure.Repositories;

public class TeacherRepository(EduDbContext context)
    : BaseRepository<Teacher, EduDbContext>(context), ITeacherRepository
{
    private readonly EduDbContext _context = context;
    public async Task<bool> EmailExistsAsync(string email, CancellationToken ct = default)
   => await _context.Users
        .AnyAsync(e => e.Email == email && !e.IsDelete, ct);

    public async Task<IEnumerable<Teacher>> GetAllWithDetailsAsync(int pageNumber, int pageSize, CancellationToken ct = default)
   => await DbSet
        .Include(t => t.User)
            .ThenInclude(u => u.Profile)
        .Where(t => !t.IsDelete)
        .Skip((pageNumber - 1) * pageSize)
        .Take(pageSize)
        .ToListAsync(ct);

    public async Task<Teacher?> GetByIdWithDetailsAsync(long id, CancellationToken ct = default)
    => await DbSet
        .Include(t => t.User)
            .ThenInclude(u => u.Profile)
        .FirstOrDefaultAsync(t => t.Id == id && !t.IsDelete, ct);
}
