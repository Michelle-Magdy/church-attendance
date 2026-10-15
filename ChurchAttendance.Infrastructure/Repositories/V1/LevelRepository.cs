using ChurchAttendance.Core.Domain.Entities;
using ChurchAttendance.Core.Domain.RepositoryContracts.V1;
using Microsoft.EntityFrameworkCore;

namespace ChurchAttendance.Infrastructure.Repositories.V1;

public class LevelRepository : ILevelRepository
{
    private readonly AppDbContext _db;

    public LevelRepository(AppDbContext db)
    {
        _db = db;
    }

    public Task<List<Level>> GetAllAsync()
        => _db.Levels.AsNoTracking().OrderBy(l => l.SortOrder).ToListAsync();
}
