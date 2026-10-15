using FluentValidation;
using ChurchAttendance.Core.DTOs;

namespace ChurchAttendance.API.Validators
{
    public class UserResponseDTOValidator : AbstractValidator<UserResponseDTO>
    {
        public UserResponseDTOValidator()
        {
            RuleFor(x => x.Id).NotEmpty();
            RuleFor(x => x.PersonName).MaximumLength(200);
            RuleFor(x => x.PhoneNumber).MaximumLength(20);
        }
    }
}
