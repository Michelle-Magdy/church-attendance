using ChurchAttendance.Core.Domain.Enums;
using System;
using System.Collections.Generic;
using System.Text;

namespace ChurchAttendance.Core.DTOs
{
    public class AddAttendanceDTO
    {
        public Guid UserId { get; set; }
        public Guid AttendanceDayId { get; set; }
        public AttendanceMethod Method { get; set; }
    }
}
