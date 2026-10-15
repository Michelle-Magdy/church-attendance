namespace ChurchAttendance.Core.ServiceContracts.V1;

public interface IAuditService
{
    Task LogAsync(string action, string targetType, string? targetId, string? details, string performedBy);
}
