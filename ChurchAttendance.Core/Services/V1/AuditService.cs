using ChurchAttendance.Core.Domain.Entities;
using ChurchAttendance.Core.Domain.RepositoryContracts.V1;
using ChurchAttendance.Core.ServiceContracts.V1;

namespace ChurchAttendance.Core.Services.V1;

public class AuditService : IAuditService
{
    private readonly IAuditLogRepository _repo;

    public AuditService(IAuditLogRepository repo)
    {
        _repo = repo;
    }

    public Task LogAsync(string action, string targetType, string? targetId, string? details, string performedBy)
        => _repo.AddAsync(new AuditLog
        {
            Id = Guid.NewGuid(),
            Action = action,
            TargetType = targetType,
            TargetId = targetId,
            Details = details,
            PerformedBy = performedBy,
            Timestamp = DateTime.UtcNow
        });
}
