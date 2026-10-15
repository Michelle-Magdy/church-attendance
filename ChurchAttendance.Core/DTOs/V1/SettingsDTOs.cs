namespace ChurchAttendance.Core.DTOs.V1;

public class ChurchSettingsDto
{
    public string? ChurchName { get; set; }
    public string? ContactPhone { get; set; }
    public string? ContactEmail { get; set; }
    public string? MaleContactWhatsAppUrl { get; set; }
    public string? MaleGroupWhatsAppUrl { get; set; }
    public string? FemaleGroupWhatsAppUrl { get; set; }
    public string? FacebookPageUrl { get; set; }
}
