
using System;
using System.Collections.Generic;
using System.Text;

namespace ChurchAttendance.Core.Domain.Entities
{
    public class AttendanceDay
    {
        public Guid AttendanceDayId { get; set; }
        public string? Title { get; set; } = string.Empty;
        public string? QrCode   { get; set; }= string.Empty;
        public DateTime Date  { get; set; }
        public bool IsOpen  { get; set; }
        public DateTime CreatedAt  { get; set; }
    }
}
