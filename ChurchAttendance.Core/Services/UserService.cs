using ChurchAttendance.Core.Domain.Entities;
using ChurchAttendance.Core.Domain.Enums;
using ChurchAttendance.Core.Domain.RepositoryContracts;
using ChurchAttendance.Core.DTOs;
using ChurchAttendance.Core.Exceptions;
using ChurchAttendance.Core.ServiceContracts;
using ChurchAttendance.Core.Shared;
using Microsoft.AspNetCore.Identity;

namespace ChurchAttendance.Core.Services;

public class UserService : IUserService
{
    private static readonly string UserRole = UserTypeOptions.User.ToString();
    private static readonly string AdminRole = UserTypeOptions.Admin.ToString();

    private readonly UserManager<ApplicationUser> _userManager;
    private readonly IUserRepository _userRepo;
    private readonly IAttendanceRepository _attendanceRepo;
    private readonly IAttendanceDayRepository _dayRepo;
    private readonly ICurrentUserService _currentUser;

    public UserService(
        UserManager<ApplicationUser> userManager,
        IUserRepository userRepo,
        IAttendanceRepository attendanceRepo,
        IAttendanceDayRepository dayRepo,
        ICurrentUserService currentUser)
    {
        _userManager = userManager;
        _userRepo = userRepo;
        _attendanceRepo = attendanceRepo;
        _dayRepo = dayRepo;
        _currentUser = currentUser;
    }

    public Task<PagedResult<UserListItemDTO>> GetPagedAsync(string? search, int page, int pageSize)
    {
        (page, pageSize) = Paging.Normalize(page, pageSize);
        return _userRepo.GetPagedAsync(search, page, pageSize);
    }

    public Task<int> GetCountAsync() => _userRepo.CountAsync();

    public async Task<UserResponseDTO> GetByIdAsync(Guid id)
    {
        var user = await GetUserAsync(id);
        return await BuildDetailsAsync(user);
    }

    public async Task<UserResponseDTO> CreateAsync(CreateUserDTO dto)
    {
        var makeAdmin = string.Equals(dto.Role, AdminRole, StringComparison.OrdinalIgnoreCase);

        var user = new ApplicationUser
        {
            PersonName = dto.PersonName?.Trim(),
            UserName = dto.PhoneNumber,
            PhoneNumber = dto.PhoneNumber,
            Confessor = dto.Confessor?.Trim(),
            Address = dto.Address?.Trim(),
            ChurchCode = dto.ChurchCode?.Trim(),
            isActive = true,
            CreatedAt = DateTime.UtcNow
        };

        var created = await _userManager.CreateAsync(user, dto.Password!);
        if (!created.Succeeded)
            throw FromIdentityErrors(created);

        var roles = makeAdmin ? new[] { UserRole, AdminRole } : new[] { UserRole };
        var roleResult = await _userManager.AddToRolesAsync(user, roles);
        if (!roleResult.Succeeded)
        {
            await _userManager.DeleteAsync(user);   // do not leave a user without a role behind
            throw FromIdentityErrors(roleResult);
        }

        return user.ToUserResponseDTO(makeAdmin ? AdminRole : UserRole);
    }

    public async Task<UserResponseDTO> UpdateAsync(Guid id, UpdateUserDTO dto)
    {
        var user = await GetUserAsync(id);

        user.PersonName = dto.PersonName?.Trim();
        user.Confessor = dto.Confessor?.Trim();
        user.Address = dto.Address?.Trim();
        user.ChurchCode = dto.ChurchCode?.Trim();

        if (!string.Equals(user.PhoneNumber, dto.PhoneNumber, StringComparison.Ordinal))
        {
            // the phone number is also the login name
            user.PhoneNumber = dto.PhoneNumber;
            user.UserName = dto.PhoneNumber;
        }

        user.UpdatedAt = DateTime.UtcNow;

        var result = await _userManager.UpdateAsync(user);
        if (!result.Succeeded)
            throw FromIdentityErrors(result);

        return await BuildDetailsAsync(user);
    }

    public async Task SetStatusAsync(Guid id, bool isActive)
    {
        if (!isActive && _currentUser.userId == id)
            throw new BadRequestException("You cannot deactivate your own account.");

        var user = await GetUserAsync(id);
        user.isActive = isActive;
        user.UpdatedAt = DateTime.UtcNow;

        if (!isActive)
        {
            // stop the user from renewing access
            user.RefreshToken = null;
            user.RefreshTokenExpirationDate = DateTime.UtcNow;
        }

        var result = await _userManager.UpdateAsync(user);
        if (!result.Succeeded)
            throw FromIdentityErrors(result);
    }

    public async Task ResetPasswordAsync(Guid id, string newPassword)
    {
        var user = await GetUserAsync(id);

        var token = await _userManager.GeneratePasswordResetTokenAsync(user);
        var result = await _userManager.ResetPasswordAsync(user, token, newPassword);
        if (!result.Succeeded)
            throw FromIdentityErrors(result);

        user.RefreshToken = null;
        user.RefreshTokenExpirationDate = DateTime.UtcNow;
        await _userManager.UpdateAsync(user);
    }

    public async Task<PagedResult<MyAttendanceDayDTO>> GetAttendanceDaysAsync(Guid id, int page, int pageSize)
    {
        var user = await GetUserAsync(id);
        (page, pageSize) = Paging.Normalize(page, pageSize);
        return await _dayRepo.GetPagedForUserAsync(user.Id, user.CreatedAt, page, pageSize);
    }

    // ------------------------------------------------------------------ helpers

    private async Task<ApplicationUser> GetUserAsync(Guid id)
        => await _userManager.FindByIdAsync(id.ToString()) ?? throw new NotFoundException("User not found.");

    private async Task<UserResponseDTO> BuildDetailsAsync(ApplicationUser user)
    {
        var roles = await _userManager.GetRolesAsync(user);
        var role = roles.Contains(AdminRole) ? AdminRole : roles.FirstOrDefault() ?? UserRole;
        var summary = await _attendanceRepo.GetUserSummaryAsync(user.Id, user.CreatedAt);
        return user.ToUserResponseDTO(role, summary);
    }

    private static AppException FromIdentityErrors(IdentityResult result)
    {
        var message = string.Join(" | ", result.Errors.Select(e => e.Description));

        return result.Errors.Any(e => e.Code is "DuplicateUserName" or "DuplicateEmail")
            ? new ConflictException(message)
            : new BadRequestException(message);
    }
}
