using FluentValidation;
using ChurchAttendance.Core.DTOs;

namespace ChurchAttendance.API.Validators
{
    public class AddAttendanceDTOValidator : AbstractValidator<AddAttendanceDTO>
    {
        public AddAttendanceDTOValidator()
        {
            RuleFor(x => x.UserId).NotEmpty().WithMessage("UserId is required.");
            RuleFor(x => x.AttendanceDayId).NotEmpty().WithMessage("AttendanceDayId is required.");
            RuleFor(x => x.Method).IsInEnum().WithMessage("Method is required.").NotEmpty();
        }
    }
}
