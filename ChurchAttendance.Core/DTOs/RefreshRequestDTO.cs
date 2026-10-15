using System;
using System.Collections.Generic;
using System.Text;

namespace ChurchAttendance.Core.DTOs
{
    public class RefreshRequestDTO
    {
        public string? Token { get; set; } = string.Empty;
        public string? RefreshToken { get; set; } = string.Empty;
    }
}
