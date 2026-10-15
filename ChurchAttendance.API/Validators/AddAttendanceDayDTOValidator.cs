using FluentValidation;
using ChurchAttendance.Core.DTOs;

namespace ChurchAttendance.API.Validators
{
    public class AddAttendanceDayDTOValidator : AbstractValidator<AddAttendanceDayDTO>
    {
        public AddAttendanceDayDTOValidator()
        {
            RuleFor(x => x.Date)
                .NotEmpty().WithMessage("Date is required.");

            RuleFor(x => x.Title)
                .MaximumLength(200).WithMessage("Title must be 200 characters or less.");
        }
    }
}
