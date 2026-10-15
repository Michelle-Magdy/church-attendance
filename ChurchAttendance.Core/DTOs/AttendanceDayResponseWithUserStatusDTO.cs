using ChurchAttendance.Core.Domain.Enums;
using System;
using System.Collections.Generic;
using System.Text;

namespace ChurchAttendance.Core.DTOs
{
    public class AttendanceDayResponseWithUserStatusDTO
    {
      
        public Guid AttendanceDayId { get; set; }
        public string Title { get; set; } = string.Empty;
        public string QrCode { get; set; }
        public DateTime Date { get; set; }
        public bool IsOpen { get; set; }
        public DateTime CreatedAt { get; set; }
        public Guid? AttendanceId { get; set; }
        public AttendanceMethod? Method { get; set; }
        public DateTime? MarkedAt { get; set; }
    }

}
