using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using Microsoft.IdentityModel.Tokens;

namespace LibraryManagementMVC.Jwt
{
    public static class JwtHelper
    {
        public static string GenerateJwt(JwtDto JwtInfo)
        {
            if (string.IsNullOrEmpty(JwtInfo.SecretKey))
            {
                throw new ArgumentException(
                    "SecretKey cannot be null or empty",
                    nameof(JwtInfo.SecretKey)
                );
            }

            if (string.IsNullOrEmpty(JwtInfo.Email))
            {
                throw new ArgumentException("Email cannot be null or empty", nameof(JwtInfo.Email));
            }

            var secretKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(JwtInfo.SecretKey));
            var credentials = new SigningCredentials(secretKey, SecurityAlgorithms.HmacSha256);

            var expireMinutes = JwtInfo.ExpireMinutes;

            var claims = new[]
            {
                new Claim("Id", JwtInfo.Id.ToString()),
                new Claim("Email", JwtInfo.Email),
                new Claim("UserRole", JwtInfo.UserType.ToString()),
                new Claim(ClaimTypes.Role, JwtInfo.UserType.ToString()),
                new Claim("ExpireTime", expireMinutes.ToString()),
            };

            var expireTime = DateTime.UtcNow.AddMinutes(JwtInfo.ExpireMinutes);

            var tokenDescriptor = new JwtSecurityToken(
                JwtInfo.Issuer,
                JwtInfo.Audience,
                claims,
                null,
                expireTime,
                credentials
            );

            var token = new JwtSecurityTokenHandler().WriteToken(tokenDescriptor);

            return token;
        }

        public static ClaimsPrincipal GetPrincipalFromExpiredToken(
            string token,
            string secretKey,
            string issuer,
            string audience
        )
        {
            var tokenHandler = new JwtSecurityTokenHandler();
            var key = Encoding.UTF8.GetBytes(secretKey);

            try
            {
                var principal = tokenHandler.ValidateToken(
                    token,
                    new TokenValidationParameters
                    {
                        ValidateIssuer = true,
                        ValidIssuer = issuer,
                        ValidateAudience = true,
                        ValidAudience = audience,
                        ValidateLifetime = false,
                        IssuerSigningKey = new SymmetricSecurityKey(key),
                    },
                    out var validatedToken
                );

                return principal;
            }
            catch
            {
                return null;
            }
        }
    }
}
