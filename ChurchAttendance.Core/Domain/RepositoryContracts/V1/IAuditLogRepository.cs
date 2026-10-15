using ChurchAttendance.Core.Domain.Entities;

namespace ChurchAttendance.Core.Domain.RepositoryContracts.V1;

public interface IAuditLogRepository
{
    Task AddAsync(AuditLog log);
    Task<List<AuditLog>> GetRecentAsync(int limit);
}
