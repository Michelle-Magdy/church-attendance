using FluentValidation;
using ChurchAttendance.Core.DTOs;

namespace ChurchAttendance.API.Validators
{
    public class RefreshRequestDTOValidator : AbstractValidator<RefreshRequestDTO>
    {
        public RefreshRequestDTOValidator()
        {
            RuleFor(x => x.Token).NotEmpty().WithMessage("Token is required.");
            RuleFor(x => x.RefreshToken).NotEmpty().WithMessage("Refresh token is required.");
        }
    }
}
