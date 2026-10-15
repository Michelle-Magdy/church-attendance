using ChurchAttendance.Core.Domain.RepositoryContracts.V1;
using ChurchAttendance.Core.DTOs.V1;
using ChurchAttendance.Core.ServiceContracts.V1;

namespace ChurchAttendance.Core.Services.V1;

public class DashboardService : IDashboardService
{
    private readonly IMemberRepository _memberRepo;
    private readonly IV1AttendanceDayRepository _dayRepo;
    private readonly IV1AttendanceRepository _attendanceRepo;
    private readonly IAuditLogRepository _auditRepo;

    public DashboardService(
        IMemberRepository memberRepo,
        IV1AttendanceDayRepository dayRepo,
        IV1AttendanceRepository attendanceRepo,
        IAuditLogRepository auditRepo)
    {
        _memberRepo = memberRepo;
        _dayRepo = dayRepo;
        _attendanceRepo = attendanceRepo;
        _auditRepo = auditRepo;
    }

    public async Task<DashboardSummaryDto> GetSummaryAsync(string? from, string? to)
    {
        DateOnly? fromDate = TryParseDate(from);
        DateOnly? toDate = TryParseDate(to);

        var allMembers = await _memberRepo.GetPagedAsync(new MemberListQueryDto { Page = 1, PageSize = int.MaxValue });
        var totalUsers = allMembers.TotalCount;
        var pendingUsers = allMembers.Items.Count(m => m.Status == 1);

        var finalizedDays = await _dayRepo.GetFinalizedInRangeAsync(fromDate, toDate);
        var totalDays = finalizedDays.Count;
        var totalAttendees = finalizedDays.Sum(d => d.TotalAttendees);
        var totalAbsentees = finalizedDays.Sum(d => d.TotalAbsentees);
        var totalEligible = finalizedDays.Sum(d => d.TotalEligibleUsers);
        var pct = totalEligible > 0 ? Math.Round((double)totalAttendees / totalEligible * 100, 1) : 0;

        var last = finalizedDays.OrderByDescending(d => d.Date).FirstOrDefault();

        return new DashboardSummaryDto
        {
            TotalUsers = totalUsers,
            TotalPendingUsers = pendingUsers,
            TotalAttendanceDays = totalDays,
            TotalAttendees = totalAttendees,
            TotalAbsentees = totalAbsentees,
            LastAttendanceDayDate = last?.Date.ToString("yyyy-MM-dd"),
            LastDayAttendees = last?.TotalAttendees ?? 0,
            LastDayAbsentees = last?.TotalAbsentees ?? 0,
            AttendancePercentage = pct
        };
    }

    public async Task<List<AttendanceTrendDto>> GetAttendanceTrendAsync(string? from, string? to)
    {
        DateOnly? fromDate = TryParseDate(from);
        DateOnly? toDate = TryParseDate(to);

        var days = await _dayRepo.GetFinalizedInRangeAsync(fromDate, toDate);
        return days
            .OrderBy(d => d.Date)
            .Select(d =>
            {
                var pct = d.TotalEligibleUsers > 0
                    ? Math.Round((double)d.TotalAttendees / d.TotalEligibleUsers * 100, 1)
                    : 0;
                return new AttendanceTrendDto
                {
                    Date = d.Date.ToString("yyyy-MM-dd"),
                    Title = d.Title,
                    Attendees = d.TotalAttendees,
                    Absentees = d.TotalAbsentees,
                    AttendancePercentage = pct
                };
            })
            .ToList();
    }

    public async Task<List<AttendanceDayDto>> GetRecentAttendanceDaysAsync(int limit)
    {
        limit = Math.Clamp(limit, 1, 50);
        var days = await _dayRepo.GetRecentAsync(limit);
        return days.Select(V1AttendanceDayService.ToDto).ToList();
    }

    public async Task<List<AuditLogDto>> GetAuditLogsAsync(int limit)
    {
        limit = Math.Clamp(limit, 1, 100);
        var logs = await _auditRepo.GetRecentAsync(limit);
        return logs.Select(l => new AuditLogDto
        {
            Id = l.Id.ToString(),
            Action = l.Action,
            TargetType = l.TargetType,
            TargetId = l.TargetId,
            Details = l.Details,
            PerformedBy = l.PerformedBy,
            Timestamp = l.Timestamp
        }).ToList();
    }

    private static DateOnly? TryParseDate(string? s)
        => DateOnly.TryParse(s, out var d) ? d : null;
}
