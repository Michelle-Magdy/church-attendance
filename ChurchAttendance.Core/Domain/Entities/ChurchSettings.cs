namespace ChurchAttendance.Core.Domain.Entities;

public class ChurchSettings
{
    public int Id { get; set; } = 1;  // single-row table
    public string? ChurchName { get; set; }
    public string? ContactPhone { get; set; }
    public string? ContactEmail { get; set; }
    public string? MaleContactWhatsAppUrl { get; set; }
    public string? MaleGroupWhatsAppUrl { get; set; }
    public string? FemaleGroupWhatsAppUrl { get; set; }
    public string? FacebookPageUrl { get; set; }
}
