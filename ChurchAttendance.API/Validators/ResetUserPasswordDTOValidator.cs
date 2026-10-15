using ChurchAttendance.Core.DTOs;
using FluentValidation;

namespace ChurchAttendance.API.Validators;

public class ResetUserPasswordDTOValidator : AbstractValidator<ResetUserPasswordDTO>
{
    public ResetUserPasswordDTOValidator()
    {
        RuleFor(x => x.NewPassword)
            .NotEmpty().WithMessage("New password is required.")
            .MinimumLength(8).WithMessage("Password must be at least 8 characters.");
    }
}
