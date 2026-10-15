using Microsoft.AspNetCore.Identity;
using System;
using System.Collections.Generic;
using System.Text;

namespace ChurchAttendance.Core.Domain.Entities
{
    public class ApplicationUser:IdentityUser<Guid>
    {
        public string? PersonName { get; set; } = string.Empty;
        public string? Confessor   { get; set; }= string.Empty;
        public string? Address { get; set; } = string.Empty;
        public string? ChurchCode   { get; set; }= string.Empty;
        public DateTime CreatedAt  { get; set; }
        public DateTime? UpdatedAt  { get; set; }
        public bool isActive { get; set; }
        public string? RefreshToken { get; set; } = string.Empty;
        public DateTime RefreshTokenExpirationDate { get; set; }

        public ICollection<Attendance> Attendances { get; set; } = new List<Attendance>();
    }
}
