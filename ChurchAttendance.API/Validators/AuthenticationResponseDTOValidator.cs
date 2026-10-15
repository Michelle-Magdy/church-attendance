using FluentValidation;
using ChurchAttendance.Core.DTOs;

namespace ChurchAttendance.API.Validators
{
    public class AuthenticationResponseDTOValidator : AbstractValidator<AuthenticationResponseDTO>
    {
        public AuthenticationResponseDTOValidator()
        {
            RuleFor(x => x.Token).NotEmpty();
            RuleFor(x => x.RefreshToken).NotEmpty();
        }
    }
}
