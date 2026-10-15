using System.Globalization;
using ChurchAttendance.Core.Domain.Entities;
using ChurchAttendance.Core.Domain.RepositoryContracts;
using ChurchAttendance.Core.DTOs;
using ChurchAttendance.Core.Exceptions;
using ChurchAttendance.Core.ServiceContracts;
using ChurchAttendance.Core.Shared;
using QRCoder;
using QuestPDF.Fluent;
using QuestPDF.Helpers;
using QuestPDF.Infrastructure;

namespace ChurchAttendance.Core.Services;

public class AttendanceDayService : IAttendanceDayService
{
    private readonly IAttendanceDayRepository _dayRepo;
    private readonly IAttendanceRepository _attendanceRepo;

    static AttendanceDayService()
    {
        // QuestPDF Community license (free for small organisations - check the QuestPDF license terms).
        QuestPDF.Settings.License = LicenseType.Community;
    }

    public AttendanceDayService(IAttendanceDayRepository dayRepo, IAttendanceRepository attendanceRepo)
    {
        _dayRepo = dayRepo;
        _attendanceRepo = attendanceRepo;
    }

    // ------------------------------------------------------------------ lists & summary

    public Task<PagedResult<AttendanceDayListItemDTO>> GetPagedAsync(int page, int pageSize)
    {
        (page, pageSize) = Paging.Normalize(page, pageSize);
        return _dayRepo.GetPagedWithCountsAsync(page, pageSize);
    }

    public async Task<AttendanceDaysSummaryDTO> GetSummaryAsync()
    {
        var total = await _dayRepo.CountAsync();
        var last = await _dayRepo.GetLastWithCountAsync();
        return new AttendanceDaysSummaryDTO { TotalDays = total, LastDay = last };
    }

    public async Task<AttendanceDayDetailsDTO> GetDetailsAsync(Guid id)
    {
        var day = await GetDayAsync(id);
        var attendees = await _attendanceRepo.CountForDayAsync(id);
        var absent = await _attendanceRepo.CountAbsentForDayAsync(id, day.Date);

        return new AttendanceDayDetailsDTO
        {
            AttendanceDayId = day.AttendanceDayId,
            Title = day.Title ?? string.Empty,
            Date = day.Date,
            IsOpen = day.IsOpen,
            TotalAttendees = attendees,
            TotalAbsent = absent,
            QrCode = day.QrCode ?? string.Empty
        };
    }

    // ------------------------------------------------------------------ commands

    public async Task<AttendanceDayResponseDTO> CreateAsync(AddAttendanceDayDTO dto)
    {
        var entity = new AttendanceDay
        {
            Date = DateTime.SpecifyKind(dto.Date, DateTimeKind.Utc),
            Title = dto.Title?.Trim(),
            IsOpen = true
        };

        var added = await _dayRepo.AddAsync(entity);
        return ToDto(added);
    }

    public async Task<AttendanceDayResponseDTO> UpdateAsync(Guid id, EditAttendanceDayDTO dto)
    {
        var day = await GetDayAsync(id);

        if (dto.Title is not null) day.Title = dto.Title.Trim();
        if (dto.Date.HasValue) day.Date = DateTime.SpecifyKind(dto.Date.Value, DateTimeKind.Utc);

        var updated = await _dayRepo.UpdateAsync(day);
        return ToDto(updated);
    }

    public async Task<AttendanceDayResponseDTO> SetStatusAsync(Guid id, bool isOpen)
    {
        var day = await GetDayAsync(id);
        day.IsOpen = isOpen;

        var updated = await _dayRepo.UpdateAsync(day);
        return ToDto(updated);
    }

    public async Task DeleteAsync(Guid id)
    {
        if (!await _dayRepo.ExistsAsync(id))
            throw new NotFoundException("Attendance day not found.");

        // The database cascades attendance rows on delete, so refuse instead of silently losing data.
        if (await _attendanceRepo.HasAnyForDayAsync(id))
            throw new ConflictException("This day already has attendances. Close it instead of deleting it.");

        await _dayRepo.DeleteAsync(id);
    }

    // ------------------------------------------------------------------ QR

    public async Task<byte[]> GetQrPngAsync(Guid id)
    {
        var day = await GetDayAsync(id);
        return GenerateQrPng(day);
    }

    public async Task<byte[]> GetQrPdfAsync(Guid id)
    {
        var day = await GetDayAsync(id);
        var png = GenerateQrPng(day);

        var title = day.Title ?? string.Empty;
        var dateText = day.Date.ToString("dddd, dd MMMM yyyy", CultureInfo.InvariantCulture);
        var rtl = ContainsArabic(title);

        return Document.Create(document =>
        {
            document.Page(page =>
            {
                page.Size(PageSizes.A4);
                page.Margin(40);
                page.DefaultTextStyle(x => x.FontFamily(Fonts.Arial));
                if (rtl) page.ContentFromRightToLeft();

                page.Content().AlignMiddle().Column(column =>
                {
                    column.Spacing(18);
                    column.Item().AlignCenter().Text(title).FontSize(32).Bold();
                    column.Item().AlignCenter().Text(dateText).FontSize(18);
                    column.Item().AlignCenter().Width(360).Image(png);
                    column.Item().AlignCenter().Text("Scan to record your attendance").FontSize(16);
                });
            });
        }).GeneratePdf();
    }

    // ------------------------------------------------------------------ attendees / absentees

    public async Task<PagedResult<AttendeeDTO>> GetAttendeesAsync(Guid id, string? search, int page, int pageSize)
    {
        await GetDayAsync(id);
        (page, pageSize) = Paging.Normalize(page, pageSize);
        return await _attendanceRepo.GetAttendeesAsync(id, search, page, pageSize);
    }

    public async Task<PagedResult<UserListItemDTO>> GetAbsenteesAsync(Guid id, string? search, int page, int pageSize)
    {
        var day = await GetDayAsync(id);
        (page, pageSize) = Paging.Normalize(page, pageSize);
        return await _attendanceRepo.GetAbsenteesAsync(id, day.Date, search, page, pageSize);
    }

    // ------------------------------------------------------------------ helpers

    private async Task<AttendanceDay> GetDayAsync(Guid id)
        => await _dayRepo.GetByIdAsync(id) ?? throw new NotFoundException("Attendance day not found.");

    private static byte[] GenerateQrPng(AttendanceDay day)
    {
        var content = string.IsNullOrEmpty(day.QrCode) ? QrCodeFormat.Build(day.AttendanceDayId) : day.QrCode;

        using var generator = new QRCodeGenerator();
        using var data = generator.CreateQrCode(content, QRCodeGenerator.ECCLevel.Q);
        using var png = new PngByteQRCode(data);
        return png.GetGraphic(20);
    }

    private static bool ContainsArabic(string text)
        => text.Any(c => c >= '\u0600' && c <= '\u06FF');

    private static AttendanceDayResponseDTO ToDto(AttendanceDay a) => new()
    {
        AttendanceDayId = a.AttendanceDayId,
        Title = a.Title,
        Date = a.Date,
        IsOpen = a.IsOpen,
        CreatedAt = a.CreatedAt
    };
}
