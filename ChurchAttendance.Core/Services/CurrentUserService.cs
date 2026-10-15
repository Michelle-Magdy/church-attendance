using ChurchAttendance.Core.ServiceContracts;
using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

namespace ChurchAttendance.Core.Services
{
    public class CurrentUserService : ICurrentUserService
    {
        private readonly IHttpContextAccessor _httpContextAccessor;

        public CurrentUserService(IHttpContextAccessor httpContextAccessor)
        {
            _httpContextAccessor = httpContextAccessor;
        }

        public Guid? userId
        {
            get
            {
                var user = _httpContextAccessor.HttpContext?.User;
                if(user?.Identity?.IsAuthenticated != true)
                {
                    return null;
                }

                var sub = user.FindFirst(JwtRegisteredClaimNames.Sub)?.Value
                    ??user.FindFirst(ClaimTypes.NameIdentifier)?.Value 
                    ?? user.FindFirst("userId")?.Value;

                return (Guid.TryParse(sub, out var userId)) ? userId : null;
            }
        }
    }
}
