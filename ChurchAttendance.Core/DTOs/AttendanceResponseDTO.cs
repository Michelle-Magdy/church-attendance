using ChurchAttendance.Core.Domain.Enums;
using System;
using System.Collections.Generic;
using System.Text;

namespace ChurchAttendance.Core.DTOs
{
    public class AttendanceResponseDTO
    {
        public Guid AttendanceId { get; set; }
        public Guid UserId { get; set; }
        public Guid AttendanceDayId { get; set; }
        public AttendanceMethod Method { get; set; }

        public DateTime MarkedAt { get; set; }
    }
}
