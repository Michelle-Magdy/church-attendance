using ChurchAttendance.Core.Domain.Enums;
using System;
using System.Collections.Generic;
using System.Text;

namespace ChurchAttendance.Core.Domain.Entities
{
    public class Attendance
    {
        public Guid AttendanceId { get; set; }
        public Guid UserId { get; set; }
        public ApplicationUser User { get; set; } = null!;
        public Guid AttendanceDayId { get; set; }
        public AttendanceDay AttendanceDay { get; set; } = null!;

        public DateTime MarkedAt { get; set; }

        public AttendanceMethod Method { get; set; }

    }
}
