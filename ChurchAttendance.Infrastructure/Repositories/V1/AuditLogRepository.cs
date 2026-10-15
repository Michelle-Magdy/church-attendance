using ChurchAttendance.Core.Domain.Entities;
using ChurchAttendance.Core.Domain.RepositoryContracts.V1;
using Microsoft.EntityFrameworkCore;

namespace ChurchAttendance.Infrastructure.Repositories.V1;

public class AuditLogRepository : IAuditLogRepository
{
    private readonly AppDbContext _db;

    public AuditLogRepository(AppDbContext db)
    {
        _db = db;
    }

    public async Task AddAsync(AuditLog log)
    {
        _db.AuditLogs.Add(log);
        await _db.SaveChangesAsync();
    }

    public Task<List<AuditLog>> GetRecentAsync(int limit)
        => _db.AuditLogs
            .AsNoTracking()
            .OrderByDescending(l => l.Timestamp)
            .Take(limit)
            .ToListAsync();
}
