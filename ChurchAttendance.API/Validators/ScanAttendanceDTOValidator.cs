using ChurchAttendance.Core.DTOs;
using FluentValidation;

namespace ChurchAttendance.API.Validators;

public class ScanAttendanceDTOValidator : AbstractValidator<ScanAttendanceDTO>
{
    public ScanAttendanceDTOValidator()
    {
        RuleFor(x => x.QrCode)
            .NotEmpty().WithMessage("QR code is required.")
            .MaximumLength(500).WithMessage("QR code is too long.");
    }
}
