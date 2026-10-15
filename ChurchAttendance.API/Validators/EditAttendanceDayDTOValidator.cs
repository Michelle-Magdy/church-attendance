using ChurchAttendance.Core.DTOs;
using FluentValidation;

namespace ChurchAttendance.API.Validators;

public class EditAttendanceDayDTOValidator : AbstractValidator<EditAttendanceDayDTO>
{
    public EditAttendanceDayDTOValidator()
    {
        RuleFor(x => x.Title)
            .MaximumLength(200).WithMessage("Title must be 200 characters or less.");

        RuleFor(x => x.Date)
            .Must(d => d is null || d.Value != default)
            .WithMessage("Date is not valid.");

        RuleFor(x => x)
            .Must(x => x.Title is not null || x.Date is not null)
            .WithMessage("Provide a title or a date to change.");
    }
}
