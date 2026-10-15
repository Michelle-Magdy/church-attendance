using ChurchAttendance.Core.Domain.Entities;
using ChurchAttendance.Core.Domain.Enums;
using ChurchAttendance.Core.Domain.RepositoryContracts.V1;
using ChurchAttendance.Core.DTOs.V1;
using ChurchAttendance.Core.Exceptions;
using ChurchAttendance.Core.ServiceContracts;
using ChurchAttendance.Core.ServiceContracts.V1;
using Microsoft.AspNetCore.Identity;

namespace ChurchAttendance.Core.Services.V1;

/// <summary>
/// Registers a new member in both the Members table (v1 domain) and the
/// AspNetUsers table (Identity) so the member can sign in with a JWT.
/// </summary>
public class V1AuthService : IV1AuthService
{
    private readonly IMemberRepository _memberRepo;
    private readonly UserManager<ApplicationUser> _userManager;
    private readonly SignInManager<ApplicationUser> _signInManager;
    private readonly IJWTService _jwtService;

    public V1AuthService(
        IMemberRepository memberRepo,
        UserManager<ApplicationUser> userManager,
        SignInManager<ApplicationUser> signInManager,
        IJWTService jwtService)
    {
        _memberRepo = memberRepo;
        _userManager = userManager;
        _signInManager = signInManager;
        _jwtService = jwtService;
    }

    public async Task<V1AuthResponseDto> RegisterAsync(V1RegisterDto dto)
    {
        // Validate phone
        if (!System.Text.RegularExpressions.Regex.IsMatch(dto.PhoneNumber ?? string.Empty, @"^01[0125]\d{8}$"))
            throw new BadRequestException("Enter a valid Egyptian phone number (e.g., 01012345678).");

        if (dto.Password != dto.ConfirmPassword)
            throw new BadRequestException("Password and confirm password don't match.");

        // Check uniqueness in both tables
        if (await _memberRepo.PhoneExistsAsync(dto.PhoneNumber!))
            throw new ConflictException("This phone number is already registered.");

        if (await _userManager.FindByNameAsync(dto.PhoneNumber!) is not null)
            throw new ConflictException("This phone number is already registered.");

        // Create Identity user
        var identityUser = new ApplicationUser
        {
            UserName = dto.PhoneNumber,
            PhoneNumber = dto.PhoneNumber,
            PersonName = dto.Name.Trim(),
            isActive = false,   // pending admin approval
            CreatedAt = DateTime.UtcNow
        };

        var result = await _userManager.CreateAsync(identityUser, dto.Password!);
        if (!result.Succeeded)
        {
            var msg = string.Join(" | ", result.Errors.Select(e => e.Description));
            throw result.Errors.Any(e => e.Code is "DuplicateUserName" or "DuplicateEmail")
                ? new ConflictException(msg) : new BadRequestException(msg);
        }

        await _userManager.AddToRoleAsync(identityUser, "User");

        // Create Member record
        var member = new Member
        {
            Id = Guid.NewGuid(),
            Name = dto.Name.Trim(),
            PhoneNumber = dto.PhoneNumber!.Trim(),
            Gender = (Gender)(dto.Gender == 0 ? 1 : dto.Gender),
            Age = dto.Age,
            LevelId = string.IsNullOrWhiteSpace(dto.LevelId) ? null : dto.LevelId,
            ConfessionFather = dto.ConfessionFather?.Trim(),
            Address = dto.Address?.Trim(),
            Status = UserStatus.Pending,
            Role = UserRole.User,
            CreatedAtUtc = DateTime.UtcNow
        };

        await _memberRepo.AddAsync(member);

        return new V1AuthResponseDto
        {
            Id = member.Id.ToString(),
            Name = member.Name,
            PhoneNumber = member.PhoneNumber,
            Status = (int)member.Status,
            Role = (int)member.Role,
            Token = string.Empty,
            TokenExpiresAtUtc = DateTime.UtcNow,
            RefreshToken = string.Empty,
            RefreshTokenExpiresAtUtc = DateTime.UtcNow
        };
    }

    public async Task<V1AuthResponseDto> LoginAsync(V1LoginDto dto)
    {
        const string wrongCredentials = "Wrong phone number or password.";

        var user = await _userManager.FindByNameAsync(dto.PhoneNumber);
        if (user is null)
            throw new BadRequestException(wrongCredentials);

        // Check the member's status — pending members can't log in
        var member = await _memberRepo.GetPagedAsync(new MemberListQueryDto
        {
            Page = 1, PageSize = 1, Search = dto.PhoneNumber
        });
        var memberRecord = member.Items.FirstOrDefault(m => m.PhoneNumber == dto.PhoneNumber);

        if (memberRecord is null || memberRecord.Status != (int)UserStatus.Active)
            throw new BadRequestException("Your account is pending approval or has been suspended.");

        var check = await _signInManager.CheckPasswordSignInAsync(user, dto.Password, lockoutOnFailure: true);
        if (check.IsLockedOut)
            throw new BadRequestException("Too many failed attempts. Try again later.");
        if (!check.Succeeded)
            throw new BadRequestException(wrongCredentials);

        var tokens = await _jwtService.CreateJWTTokenAsync(user);
        user.RefreshToken = tokens.RefreshToken;
        user.RefreshTokenExpirationDate = tokens.RefreshTokenExpirationDate;
        await _userManager.UpdateAsync(user);

        return new V1AuthResponseDto
        {
            Id = memberRecord.Id,
            Name = memberRecord.Name,
            PhoneNumber = memberRecord.PhoneNumber,
            Status = memberRecord.Status,
            Role = memberRecord.Role,
            Token = tokens.Token ?? string.Empty,
            TokenExpiresAtUtc = tokens.TokenExpirationDate,
            RefreshToken = tokens.RefreshToken ?? string.Empty,
            RefreshTokenExpiresAtUtc = tokens.RefreshTokenExpirationDate
        };
    }
}
