using System.Text;
using ChurchAttendance.Core.Domain.RepositoryContracts.V1;
using ChurchAttendance.Core.DTOs.V1;
using ChurchAttendance.Core.ServiceContracts.V1;
using ChurchAttendance.Core.Shared;

namespace ChurchAttendance.Core.Services.V1;

public class ReportService : IReportService
{
    private readonly IV1AttendanceDayRepository _dayRepo;
    private readonly IV1AttendanceRepository _attendanceRepo;

    public ReportService(IV1AttendanceDayRepository dayRepo, IV1AttendanceRepository attendanceRepo)
    {
        _dayRepo = dayRepo;
        _attendanceRepo = attendanceRepo;
    }

    public async Task<ReportSummaryDto> GetSummaryAsync(string? from, string? to, string? levelId, string? status)
    {
        var fromDate = TryParseDate(from);
        var toDate = TryParseDate(to);

        var rows = await _attendanceRepo.GetReportRowsAsync(fromDate, toDate, levelId, status);
        var finalizedDays = await _dayRepo.GetFinalizedInRangeAsync(fromDate, toDate);

        var totalSessions = rows.Select(r => r.AttendanceDayId).Distinct().Count();
        var finalizedSessions = finalizedDays.Count;
        var totalAttendees = rows.Sum(r => r.Attendees);
        var totalAbsentees = rows.Sum(r => r.Absentees);
        var totalEligible = rows.Sum(r => r.EligibleCount);
        var pct = totalEligible > 0 ? Math.Round((double)totalAttendees / totalEligible * 100, 1) : 0;

        // Members needing follow-up (default threshold 30%)
        var followUp = await _attendanceRepo.GetAbsenteeFollowUpAsync(fromDate, toDate, levelId, 0.3);

        return new ReportSummaryDto
        {
            From = from,
            To = to,
            TotalSessions = totalSessions,
            FinalizedSessions = finalizedSessions,
            TotalAttendees = totalAttendees,
            TotalAbsentees = totalAbsentees,
            AttendancePercentage = pct,
            MembersNeedingFollowUp = followUp.Count
        };
    }

    public async Task<PagedResult<AttendanceReportRowDto>> GetAttendanceReportAsync(
        string? from, string? to, string? levelId, string? status, int page, int pageSize)
    {
        var fromDate = TryParseDate(from);
        var toDate = TryParseDate(to);

        var rows = await _attendanceRepo.GetReportRowsAsync(fromDate, toDate, levelId, status);
        var total = rows.Count;
        var pg = Math.Max(1, page);
        var ps = Math.Clamp(pageSize, 1, 100);

        var items = rows.Skip((pg - 1) * ps).Take(ps).ToList();

        return new PagedResult<AttendanceReportRowDto>
        {
            Items = items,
            Page = pg,
            PageSize = ps,
            TotalCount = total
        };
    }

    public async Task<List<AbsenteeFollowUpDto>> GetAbsenteeFollowUpAsync(
        string? from, string? to, string? levelId, double minimumAbsenceRate)
    {
        var fromDate = TryParseDate(from);
        var toDate = TryParseDate(to);
        return await _attendanceRepo.GetAbsenteeFollowUpAsync(fromDate, toDate, levelId, minimumAbsenceRate);
    }

    public async Task<byte[]> ExportAttendanceCsvAsync(string? from, string? to, string? levelId, string? status)
    {
        var fromDate = TryParseDate(from);
        var toDate = TryParseDate(to);
        var rows = await _attendanceRepo.GetReportRowsAsync(fromDate, toDate, levelId, status);

        var sb = new StringBuilder();
        // UTF-8 BOM for correct Arabic display in Excel
        sb.Append('\uFEFF');
        sb.AppendLine("تاريخ الجلسة,عنوان الجلسة,المستوى,المؤهلون,الحاضرون,الغائبون,نسبة الحضور");

        foreach (var r in rows)
        {
            sb.AppendLine(
                $"{CsvField(r.Date)},{CsvField(r.Title)},{CsvField(r.LevelName)}," +
                $"{r.EligibleCount},{r.Attendees},{r.Absentees},{r.AttendancePercentage}%");
        }

        return Encoding.UTF8.GetBytes(sb.ToString());
    }

    private static string CsvField(string? value)
    {
        if (value is null) return string.Empty;
        // Wrap in quotes if contains comma, quote, or newline
        if (value.Contains(',') || value.Contains('"') || value.Contains('\n'))
            return $"\"{value.Replace("\"", "\"\"")}\"";
        return value;
    }

    private static DateOnly? TryParseDate(string? s)
        => DateOnly.TryParse(s, out var d) ? d : null;
}
