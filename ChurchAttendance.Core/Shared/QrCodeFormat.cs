namespace ChurchAttendance.Core.Shared;

/// <summary>The text encoded inside an attendance day's QR code.</summary>
public static class QrCodeFormat
{
    public const string Prefix = "church-attendance:";

    public static string Build(Guid attendanceDayId) => $"{Prefix}{attendanceDayId}";

    public static bool TryParse(string? value, out Guid attendanceDayId)
    {
        attendanceDayId = Guid.Empty;
        if (string.IsNullOrWhiteSpace(value)) return false;

        var text = value.Trim();
        return text.StartsWith(Prefix, StringComparison.Ordinal)
            && Guid.TryParse(text.AsSpan(Prefix.Length), out attendanceDayId);
    }
}
