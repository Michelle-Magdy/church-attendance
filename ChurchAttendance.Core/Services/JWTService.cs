using ChurchAttendance.Core.Domain.Entities;
using ChurchAttendance.Core.DTOs;
using ChurchAttendance.Core.Options;
using ChurchAttendance.Core.ServiceContracts;
using Microsoft.Extensions.Options;
using Microsoft.AspNetCore.Identity;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Security.Cryptography;
using System.Text;

namespace ChurchAttendance.Core.Services
{
    public class JWTService : IJWTService
    {
        private readonly JWTSettings _jwtOptions;
        private readonly RefreshTokenOptions _refreshTokenOptions;

        private readonly UserManager<ApplicationUser> _userManager;

        public JWTService(IOptions<JWTSettings> settingsOptions ,IOptions<RefreshTokenOptions> refreshTokenOptions, UserManager<ApplicationUser> userManager)
        {
            _jwtOptions = settingsOptions.Value;
            _refreshTokenOptions = refreshTokenOptions.Value;
            _userManager = userManager;

        }

        public async Task<AuthenticationResponseDTO> CreateJWTTokenAsync(ApplicationUser user)
        {
            if (user == null)
                throw new ArgumentNullException(nameof(user));

            DateTime expirationDate = DateTime.UtcNow.AddMinutes(Convert.ToDouble(_jwtOptions.Expiration_Mins));

            var claims = new List<Claim>
            {
                new Claim(JwtRegisteredClaimNames.Sub, user.Id.ToString()),
                new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString()),
                new Claim(
                    JwtRegisteredClaimNames.Iat,
                    EpochTime.GetIntDate(DateTime.UtcNow).ToString(),
                    ClaimValueTypes.Integer64),
                new Claim(JwtRegisteredClaimNames.Name, user.PersonName ?? string.Empty),
                new Claim(JwtRegisteredClaimNames.PhoneNumber, user.PhoneNumber ?? string.Empty)
            };

            // add role claims
            var roles = await _userManager.GetRolesAsync(user);
            foreach (var role in roles)
            {
                claims.Add(new Claim(ClaimTypes.Role, role));
            }

            var symmetricSecurityKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_jwtOptions.Key!));
            var signingCredentials = new SigningCredentials(symmetricSecurityKey, SecurityAlgorithms.HmacSha256);

            var tokenGenerator = new JwtSecurityToken(
                issuer: _jwtOptions.Issuer,
                audience: _jwtOptions.Audience,
                claims: claims,
                expires: expirationDate,
                signingCredentials: signingCredentials
            );

            var tokenHandler = new JwtSecurityTokenHandler();
            string token = tokenHandler.WriteToken(tokenGenerator);

            return new AuthenticationResponseDTO
            {
                Token = token,
                PersonName = user.PersonName,
                TokenExpirationDate = expirationDate,
                PhoneNumber = user.PhoneNumber,
                RefreshToken = GenerateRefreshToken(),
                RefreshTokenExpirationDate = DateTime.UtcNow.AddMinutes(Convert.ToDouble(_refreshTokenOptions.Expiration_Mins)),
                Role = roles.Contains("Admin") ? "Admin" : roles.FirstOrDefault()

            };
        }

        public ClaimsPrincipal? GetClaimsPrincipal(string? token)
        {
            // create token validation parameters
            var tokenValidationParameters = new TokenValidationParameters()
            {
                ValidateIssuer = true,
                ValidIssuer = _jwtOptions.Issuer,
                ValidateAudience = true,
                ValidAudience = _jwtOptions.Audience,
                ValidateLifetime = false,
                ValidateIssuerSigningKey = true,
                IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_jwtOptions.Key!)),
            };

            // create jwt securtiy token handler
            var jwtHandler = new JwtSecurityTokenHandler();
            
            // validate token and get claims principal
            ClaimsPrincipal claimsPrincipal = jwtHandler.ValidateToken(token, tokenValidationParameters, out SecurityToken validatedToken);

            // check that validated token is a jwtSecurityToken and the hashing algorithm is HmacSha256
            if(validatedToken is not JwtSecurityToken jwtSecurityToken || !jwtSecurityToken.Header.Alg.Equals(SecurityAlgorithms.HmacSha256,StringComparison.InvariantCultureIgnoreCase))
            {
                throw new SecurityTokenException("Invalid Token");
            }

            return claimsPrincipal;

        }

        private string GenerateRefreshToken()
        {
            byte[] bytes = new byte[64];
            using ( var rng = RandomNumberGenerator.Create())
            {
                rng.GetBytes(bytes);
                return Convert.ToBase64String(bytes);
            }

        }
    }
}