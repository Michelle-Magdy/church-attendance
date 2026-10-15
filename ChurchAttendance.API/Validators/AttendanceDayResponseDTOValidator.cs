using FluentValidation;
using ChurchAttendance.Core.DTOs;

namespace ChurchAttendance.API.Validators
{
    public class AttendanceDayResponseDTOValidator : AbstractValidator<AttendanceDayResponseDTO>
    {
        public AttendanceDayResponseDTOValidator()
        {
            RuleFor(x => x.AttendanceDayId).NotEmpty();
            RuleFor(x => x.Date).NotEmpty();
            RuleFor(x => x.Title).MaximumLength(200);
        }
    }
}
