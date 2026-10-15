using ChurchAttendance.Core.Domain.Entities;
using ChurchAttendance.Core.Domain.Enums;
using ChurchAttendance.Core.Domain.RepositoryContracts.V1;
using ChurchAttendance.Core.DTOs.V1;
using ChurchAttendance.Core.Exceptions;
using ChurchAttendance.Core.ServiceContracts.V1;
using ChurchAttendance.Core.Shared;

namespace ChurchAttendance.Core.Services.V1;

public class V1AttendanceDayService : IV1AttendanceDayService
{
    private readonly IV1AttendanceDayRepository _dayRepo;
    private readonly IV1AttendanceRepository _attendanceRepo;
    private readonly IMemberRepository _memberRepo;
    private readonly IAuditService _audit;

    public V1AttendanceDayService(
        IV1AttendanceDayRepository dayRepo,
        IV1AttendanceRepository attendanceRepo,
        IMemberRepository memberRepo,
        IAuditService audit)
    {
        _dayRepo = dayRepo;
        _attendanceRepo = attendanceRepo;
        _memberRepo = memberRepo;
        _audit = audit;
    }

    public async Task<PagedResult<AttendanceDayDto>> GetPagedAsync(AttendanceDayListQueryDto query)
    {
        var result = await _dayRepo.GetPagedAsync(query);
        return new PagedResult<AttendanceDayDto>
        {
            Items = result.Items.Select(ToDto).ToList(),
            Page = result.Page,
            PageSize = result.PageSize,
            TotalCount = result.TotalCount
        };
    }

    public async Task<AttendanceDayDto> GetByIdAsync(Guid id)
    {
        var day = await _dayRepo.GetByIdAsync(id)
            ?? throw new NotFoundException("Attendance day not found.");
        return ToDto(day);
    }

    public async Task<AttendanceDayDto> CreateAsync(CreateAttendanceDayDto dto, string performedBy)
    {
        if (!DateOnly.TryParse(dto.Date, out var date))
            throw new BadRequestException("Invalid date format. Use YYYY-MM-DD.");

        if (dto.EndAt.HasValue && dto.EndAt <= dto.StartAt)
            throw new BadRequestException("endAt must be later than startAt.");

        var status = dto.IsOpen ? AttendanceDayStatus.Open : AttendanceDayStatus.Scheduled;
        string? qrToken = dto.IsOpen ? GenerateQrToken() : null;
        DateTime? qrExpiry = dto.IsOpen ? DateTime.UtcNow.AddHours(4) : null;

        var day = new V1AttendanceDay
        {
            Id = Guid.NewGuid(),
            Title = dto.Title.Trim(),
            Date = date,
            StartAt = dto.StartAt,
            EndAt = dto.EndAt,
            Location = dto.Location?.Trim(),
            Notes = dto.Notes?.Trim(),
            Status = status,
            QrToken = qrToken,
            QrTokenExpiresAtUtc = qrExpiry,
            CreatedAtUtc = DateTime.UtcNow
        };

        var added = await _dayRepo.AddAsync(day);

        await _audit.LogAsync(
            "إنشاء جلسة حضور",
            nameof(V1AttendanceDay),
            added.Id.ToString(),
            $"تم إنشاء جلسة «{added.Title}»",
            performedBy);

        return ToDto(added);
    }

    public async Task<AttendanceDayDto> UpdateAsync(Guid id, UpdateAttendanceDayDto dto, string performedBy)
    {
        var day = await GetDayOrThrowAsync(id);

        if (dto.Title is not null) day.Title = dto.Title.Trim();
        if (dto.Date is not null)
        {
            if (!DateOnly.TryParse(dto.Date, out var date))
                throw new BadRequestException("Invalid date format. Use YYYY-MM-DD.");
            day.Date = date;
        }
        if (dto.StartAt.HasValue) day.StartAt = dto.StartAt.Value;
        if (dto.EndAt.HasValue) day.EndAt = dto.EndAt.Value;
        if (dto.Location is not null) day.Location = dto.Location.Trim();
        if (dto.Notes is not null) day.Notes = dto.Notes.Trim();

        if (day.EndAt.HasValue && day.EndAt <= day.StartAt)
            throw new BadRequestException("endAt must be later than startAt.");

        await _dayRepo.UpdateAsync(day);
        return ToDto(day);
    }

    public async Task<AttendanceDayDto> OpenAsync(Guid id, string performedBy)
    {
        var day = await GetDayOrThrowAsync(id);

        day.Status = AttendanceDayStatus.Open;
        day.QrToken = GenerateQrToken();
        day.QrTokenExpiresAtUtc = DateTime.UtcNow.AddHours(4);

        await _dayRepo.UpdateAsync(day);
        await _audit.LogAsync("فتح جلسة حضور", nameof(V1AttendanceDay), id.ToString(), $"تم فتح جلسة «{day.Title}»", performedBy);
        return ToDto(day);
    }

    public async Task<AttendanceDayDto> CloseAsync(Guid id, string performedBy)
    {
        var day = await GetDayOrThrowAsync(id);

        day.Status = AttendanceDayStatus.Closed;
        await _dayRepo.UpdateAsync(day);
        await _audit.LogAsync("إغلاق جلسة حضور", nameof(V1AttendanceDay), id.ToString(), $"تم إغلاق جلسة «{day.Title}»", performedBy);
        return ToDto(day);
    }

    public async Task<AttendanceDayDto> FinalizeAsync(Guid id, string performedBy)
    {
        var day = await GetDayOrThrowAsync(id);

        var eligible = await _attendanceRepo.CountEligibleAsync(id, day.Date);
        var attendees = await _attendanceRepo.CountAttendeesAsync(id);
        var absentees = eligible - attendees;
        var percentage = eligible > 0 ? Math.Round((double)attendees / eligible * 100, 1) : 0;

        day.Status = AttendanceDayStatus.Finalized;
        day.TotalEligibleUsers = eligible;
        day.TotalAttendees = attendees;
        day.TotalAbsentees = Math.Max(0, absentees);
        day.FinalizedAtUtc = DateTime.UtcNow;

        await _dayRepo.UpdateAsync(day);
        await _audit.LogAsync("تأكيد جلسة حضور", nameof(V1AttendanceDay), id.ToString(), $"تم تأكيد جلسة «{day.Title}»", performedBy);
        return ToDto(day);
    }

    public async Task<RefreshQrResponseDto> RefreshQrAsync(Guid id)
    {
        var day = await GetDayOrThrowAsync(id);

        if (day.Status != AttendanceDayStatus.Open)
            throw new BusinessRuleException("QR token can only be refreshed for open sessions.");

        day.QrToken = GenerateQrToken();
        day.QrTokenExpiresAtUtc = DateTime.UtcNow.AddHours(4);

        await _dayRepo.UpdateAsync(day);
        return new RefreshQrResponseDto
        {
            QrToken = day.QrToken,
            QrTokenExpiresAtUtc = day.QrTokenExpiresAtUtc!.Value
        };
    }

    public async Task DeleteAsync(Guid id, string performedBy)
    {
        var day = await GetDayOrThrowAsync(id);
        await _dayRepo.DeleteAsync(id);
        await _audit.LogAsync("حذف جلسة حضور", nameof(V1AttendanceDay), id.ToString(), $"تم حذف جلسة «{day.Title}»", performedBy);
    }

    public Task<List<AttendanceListItemDto>> GetAttendeesAsync(Guid dayId, string? search)
        => _attendanceRepo.GetAttendeesAsync(dayId, search);

    public Task<List<AttendanceListItemDto>> GetAbsenteesAsync(Guid dayId, string? search)
        => _attendanceRepo.GetAbsenteesAsync(dayId, search);

    public async Task<ManualAttendanceResponseDto> AddManualAttendanceAsync(Guid dayId, ManualAttendanceDto dto, string performedBy)
    {
        if (!Guid.TryParse(dto.UserId, out var memberId))
            throw new BadRequestException("Invalid userId.");

        var day = await GetDayOrThrowAsync(dayId);

        var member = await _memberRepo.GetByIdAsync(memberId)
            ?? throw new NotFoundException("Member not found.");

        if (member.IsDeleted || member.Status != Domain.Enums.UserStatus.Active)
            throw new BusinessRuleException("Only active members can be marked as present.");

        // Check duplicate
        if (await _attendanceRepo.GetByMemberAndDayAsync(memberId, dayId) is not null)
            throw new ConflictException("Attendance is already recorded for this member and session.");

        var attendance = new V1Attendance
        {
            Id = Guid.NewGuid(),
            MemberId = memberId,
            AttendanceDayId = dayId,
            MarkedAtUtc = DateTime.UtcNow,
            Method = Domain.Enums.V1AttendanceMethod.Manual,
            Notes = dto.Reason?.Trim()
        };

        var added = await _attendanceRepo.AddAsync(attendance);

        await _audit.LogAsync(
            "تسجيل حضور يدوي",
            nameof(V1Attendance),
            added.Id.ToString(),
            $"تم تسجيل حضور {member.Name} في جلسة «{day.Title}»",
            performedBy);

        return new ManualAttendanceResponseDto
        {
            Id = added.Id.ToString(),
            UserId = memberId.ToString(),
            AttendanceDayId = dayId.ToString(),
            MarkedAtUtc = added.MarkedAtUtc,
            Method = (int)added.Method,
            Notes = added.Notes
        };
    }

    public async Task RemoveAttendanceAsync(Guid dayId, Guid userId, string? reason, string performedBy)
    {
        var day = await GetDayOrThrowAsync(dayId);

        var deleted = await _attendanceRepo.DeleteByMemberAndDayAsync(userId, dayId);
        if (!deleted)
            throw new NotFoundException("Attendance record not found.");

        await _audit.LogAsync(
            "حذف تسجيل حضور",
            nameof(V1Attendance),
            null,
            $"تم حذف تسجيل الحضور في جلسة «{day.Title}». السبب: {reason}",
            performedBy);
    }

    // ---- helpers ----

    private async Task<V1AttendanceDay> GetDayOrThrowAsync(Guid id)
        => await _dayRepo.GetByIdAsync(id) ?? throw new NotFoundException("Attendance day not found.");

    private static string GenerateQrToken()
        => $"ECC-{Guid.NewGuid():N}".ToUpper();

    internal static AttendanceDayDto ToDto(V1AttendanceDay d)
    {
        var attendees = d.TotalAttendees;
        var eligible = d.TotalEligibleUsers;
        var percentage = eligible > 0 ? Math.Round((double)attendees / eligible * 100, 1) : 0;

        return new AttendanceDayDto
        {
            Id = d.Id.ToString(),
            Title = d.Title,
            Date = d.Date.ToString("yyyy-MM-dd"),
            StartAt = d.StartAt,
            EndAt = d.EndAt,
            Status = d.Status.ToString(),
            IsOpen = d.IsOpen,
            Location = d.Location,
            Notes = d.Notes,
            TotalEligibleUsers = eligible,
            TotalAttendees = attendees,
            TotalAbsentees = d.TotalAbsentees,
            AttendancePercentage = percentage,
            QrToken = d.QrToken,
            QrTokenExpiresAtUtc = d.QrTokenExpiresAtUtc,
            FinalizedAtUtc = d.FinalizedAtUtc
        };
    }
}
