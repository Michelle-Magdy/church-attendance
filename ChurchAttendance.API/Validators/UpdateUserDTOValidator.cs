using ChurchAttendance.Core.DTOs;
using FluentValidation;

namespace ChurchAttendance.API.Validators;

public class UpdateUserDTOValidator : AbstractValidator<UpdateUserDTO>
{
    public UpdateUserDTOValidator()
    {
        RuleFor(x => x.PhoneNumber)
            .NotEmpty().WithMessage("Phone number is required.")
            .Matches(@"^01[0125]\d{8}$").WithMessage("Enter a valid Egyptian phone number (e.g., 01012345678)");

        RuleFor(x => x.PersonName)
            .NotEmpty().WithMessage("Person name is required.")
            .MaximumLength(200).WithMessage("Person name cannot be greater than 200 chars");

        RuleFor(x => x.Confessor)
            .NotEmpty().WithMessage("Confessor is required.")
            .MaximumLength(200).WithMessage("Confessor cannot be greater than 200 chars");

        RuleFor(x => x.Address)
            .NotEmpty().WithMessage("Address is required.")
            .MaximumLength(200).WithMessage("Address cannot be greater than 200 chars");

        RuleFor(x => x.ChurchCode)
            .MaximumLength(100).WithMessage("Church code cannot be greater than 100 chars");
    }
}
