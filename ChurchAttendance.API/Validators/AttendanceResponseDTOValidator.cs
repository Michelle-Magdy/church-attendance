using FluentValidation;
using ChurchAttendance.Core.DTOs;

namespace ChurchAttendance.API.Validators
{
    public class AttendanceResponseDTOValidator : AbstractValidator<AttendanceResponseDTO>
    {
        public AttendanceResponseDTOValidator()
        {
            RuleFor(x => x.AttendanceId).NotEmpty();
            RuleFor(x => x.UserId).NotEmpty();
            RuleFor(x => x.AttendanceDayId).NotEmpty();
            RuleFor(x => x.MarkedAt).NotEmpty();
        }
    }
}
