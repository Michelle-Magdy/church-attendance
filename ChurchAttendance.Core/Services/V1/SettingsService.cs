using ChurchAttendance.Core.Domain.RepositoryContracts.V1;
using ChurchAttendance.Core.DTOs.V1;
using ChurchAttendance.Core.Exceptions;
using ChurchAttendance.Core.ServiceContracts.V1;

namespace ChurchAttendance.Core.Services.V1;

public class SettingsService : ISettingsService
{
    private readonly ISettingsRepository _repo;

    public SettingsService(ISettingsRepository repo)
    {
        _repo = repo;
    }

    public async Task<ChurchSettingsDto> GetAsync()
    {
        var s = await _repo.GetAsync();
        return ToDto(s);
    }

    public async Task<ChurchSettingsDto> UpdateAsync(ChurchSettingsDto dto)
    {
        ValidateSettings(dto);

        var s = await _repo.GetAsync();
        s.ChurchName = dto.ChurchName?.Trim();
        s.ContactPhone = dto.ContactPhone?.Trim();
        s.ContactEmail = dto.ContactEmail?.Trim();
        s.MaleContactWhatsAppUrl = dto.MaleContactWhatsAppUrl?.Trim();
        s.MaleGroupWhatsAppUrl = dto.MaleGroupWhatsAppUrl?.Trim();
        s.FemaleGroupWhatsAppUrl = dto.FemaleGroupWhatsAppUrl?.Trim();
        s.FacebookPageUrl = dto.FacebookPageUrl?.Trim();

        await _repo.UpdateAsync(s);
        return ToDto(s);
    }

    private static void ValidateSettings(ChurchSettingsDto dto)
    {
        if (!string.IsNullOrWhiteSpace(dto.ContactEmail) &&
            !System.Net.Mail.MailAddress.TryCreate(dto.ContactEmail, out _))
            throw new BadRequestException("Invalid email address.");

        foreach (var url in new[] { dto.MaleContactWhatsAppUrl, dto.MaleGroupWhatsAppUrl, dto.FemaleGroupWhatsAppUrl, dto.FacebookPageUrl })
        {
            if (!string.IsNullOrWhiteSpace(url) && !Uri.TryCreate(url, UriKind.Absolute, out _))
                throw new BadRequestException($"Invalid URL: {url}");
        }
    }

    private static ChurchSettingsDto ToDto(Domain.Entities.ChurchSettings s) => new()
    {
        ChurchName = s.ChurchName,
        ContactPhone = s.ContactPhone,
        ContactEmail = s.ContactEmail,
        MaleContactWhatsAppUrl = s.MaleContactWhatsAppUrl,
        MaleGroupWhatsAppUrl = s.MaleGroupWhatsAppUrl,
        FemaleGroupWhatsAppUrl = s.FemaleGroupWhatsAppUrl,
        FacebookPageUrl = s.FacebookPageUrl
    };
}
