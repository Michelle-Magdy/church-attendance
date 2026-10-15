using ChurchAttendance.Core.DTOs;
using FluentValidation;

namespace ChurchAttendance.API.Validators;

public class AddManualAttendanceDTOValidator : AbstractValidator<AddManualAttendanceDTO>
{
    public AddManualAttendanceDTOValidator()
    {
        RuleFor(x => x.UserId).NotEmpty().WithMessage("UserId is required.");
    }
}
