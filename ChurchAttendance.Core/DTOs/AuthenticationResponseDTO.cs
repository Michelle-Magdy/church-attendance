using System;
using System.Collections.Generic;
using System.Text;

namespace ChurchAttendance.Core.DTOs
{
    public class AuthenticationResponseDTO
    {
        public string? PersonName { get; set; } = string.Empty;
        public string? PhoneNumber { get; set; } = string.Empty;
        public string? Role { get; set; } = string.Empty;
        public string? Token { get; set; } = string.Empty;
        public DateTime TokenExpirationDate { get; set; }
        public string? RefreshToken { get; set; } = string.Empty;
        public DateTime RefreshTokenExpirationDate { get; set; }
    }
}
