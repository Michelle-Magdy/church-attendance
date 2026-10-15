using ChurchAttendance.Core.Domain.Entities;
using ChurchAttendance.Core.DTOs;
using System;
using System.Collections.Generic;
using System.Security.Claims;
using System.Text;

namespace ChurchAttendance.Core.ServiceContracts
{
    public interface IJWTService
    {
        Task<AuthenticationResponseDTO> CreateJWTTokenAsync(ApplicationUser user);
        ClaimsPrincipal? GetClaimsPrincipal(string? token);
    }
}
