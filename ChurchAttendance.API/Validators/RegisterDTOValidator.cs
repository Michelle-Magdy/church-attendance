using FluentValidation;
using ChurchAttendance.Core.DTOs;

namespace ChurchAttendance.API.Validators
{
    public class RegisterDTOValidator : AbstractValidator<RegisterDTO>
    {
        public RegisterDTOValidator()
        {
            RuleFor(x => x.PhoneNumber)
                .NotEmpty().WithMessage("Phone number is required.")
                .Matches("^01[0125]\\d{8}$").WithMessage("Enter a valid Egyptian phone number (e.g., 01012345678)");

            RuleFor(x => x.Password)
                .NotEmpty().WithMessage("Password is required.")
                .MinimumLength(8).WithMessage("Password must be at least 8 characters.");

            RuleFor(x => x.ConfirmPassword)
                .NotEmpty().WithMessage("Confirm Password is required.")
                .Equal(x => x.Password).WithMessage("Password and confirm password didn't match");

            RuleFor(x => x.PersonName)
                .NotEmpty().WithMessage("Person name is required.")
                .MaximumLength(200).WithMessage("Person Name cannot be greater than 200 chars");

            RuleFor(x => x.Confessor)
                .NotEmpty().WithMessage("Confessor is required.")
                .MaximumLength(200).WithMessage("Confessor cannot be greater than 200 chars");

            RuleFor(x => x.Address)
                .NotEmpty().WithMessage("Address is required.")
                .MaximumLength(200).WithMessage("Address cannot be greater than 200 chars");
        }
    }
}
