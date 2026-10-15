using ChurchAttendance.Core.Domain.Entities;
using ChurchAttendance.Core.Domain.Enums;
using ChurchAttendance.Core.Domain.RepositoryContracts;
using ChurchAttendance.Core.DTOs;
using ChurchAttendance.Core.Exceptions;
using ChurchAttendance.Core.ServiceContracts;
using ChurchAttendance.Core.Shared;
using Microsoft.AspNetCore.Identity;

namespace ChurchAttendance.Core.Services;

public class AttendanceService : IAttendanceService
{
    private readonly IAttendanceRepository _attendanceRepo;
    private readonly IAttendanceDayRepository _dayRepo;
    private readonly ICurrentUserService _currentUser;
    private readonly UserManager<ApplicationUser> _userManager;

    public AttendanceService(
        IAttendanceRepository attendanceRepo,
        IAttendanceDayRepository dayRepo,
        ICurrentUserService currentUser,
        UserManager<ApplicationUser> userManager)
    {
        _attendanceRepo = attendanceRepo;
        _dayRepo = dayRepo;
        _currentUser = currentUser;
        _userManager = userManager;
    }

    // ------------------------------------------------------------------ current user

    public async Task<AttendanceSummaryDTO> GetMySummaryAsync()
    {
        var user = await GetCurrentUserAsync();
        return await _attendanceRepo.GetUserSummaryAsync(user.Id, user.CreatedAt);
    }

    public async Task<PagedResult<MyAttendanceDayDTO>> GetMyDaysAsync(int page, int pageSize)
    {
        var user = await GetCurrentUserAsync();
        (page, pageSize) = Paging.Normalize(page, pageSize);
        return await _dayRepo.GetPagedForUserAsync(user.Id, user.CreatedAt, page, pageSize);
    }

    public async Task<ScanAttendanceResponseDTO> ScanAsync(string? qrCode)
    {
        var user = await GetCurrentUserAsync();

        if (!QrCodeFormat.TryParse(qrCode, out var dayId))
            throw new BadRequestException("Invalid QR code.");

        var day = await _dayRepo.GetByIdAsync(dayId)
            ?? throw new NotFoundException("Attendance day not found.");

        // The scanned text must be exactly the code stored for that day.
        if (!string.Equals(day.QrCode, qrCode!.Trim(), StringComparison.Ordinal))
            throw new BadRequestException("Invalid QR code.");

        if (!day.IsOpen)
            throw new BusinessRuleException("Attendance for this day is closed.");

        // Method and user are decided by the server, never by the client.
        var attendance = await _attendanceRepo.TryAddAsync(new Attendance
        {
            UserId = user.Id,
            AttendanceDayId = day.AttendanceDayId,
            Method = AttendanceMethod.QrCode
        }) ?? throw new ConflictException("Your attendance is already recorded for this day.");

        return new ScanAttendanceResponseDTO
        {
            AttendanceId = attendance.AttendanceId,
            AttendanceDayId = day.AttendanceDayId,
            Title = day.Title ?? string.Empty,
            Date = day.Date,
            MarkedAt = attendance.MarkedAt
        };
    }

    // ------------------------------------------------------------------ admin

    public async Task<AttendanceResponseDTO> AddManualAsync(Guid attendanceDayId, Guid userId)
    {
        if (!await _dayRepo.ExistsAsync(attendanceDayId))
            throw new NotFoundException("Attendance day not found.");

        var user = await _userManager.FindByIdAsync(userId.ToString())
            ?? throw new NotFoundException("User not found.");

        // Admins may record attendance manually even when the day is closed.
        var attendance = await _attendanceRepo.TryAddAsync(new Attendance
        {
            UserId = user.Id,
            AttendanceDayId = attendanceDayId,
            Method = AttendanceMethod.Manual
        }) ?? throw new ConflictException("This user's attendance is already recorded for this day.");

        return new AttendanceResponseDTO
        {
            AttendanceId = attendance.AttendanceId,
            UserId = attendance.UserId,
            AttendanceDayId = attendance.AttendanceDayId,
            Method = attendance.Method,
            MarkedAt = attendance.MarkedAt
        };
    }

    public async Task DeleteAsync(Guid attendanceId)
    {
        if (!await _attendanceRepo.DeleteAsync(attendanceId))
            throw new NotFoundException("Attendance not found.");
    }

    // ------------------------------------------------------------------ helpers

    private async Task<ApplicationUser> GetCurrentUserAsync()
    {
        var id = _currentUser.userId ?? throw new UnauthorizedAccessException("User is not authenticated.");

        var user = await _userManager.FindByIdAsync(id.ToString())
            ?? throw new UnauthorizedAccessException("User no longer exists.");

        if (!user.isActive)
            throw new ForbiddenException("Your account is deactivated.");

        return user;
    }
}
