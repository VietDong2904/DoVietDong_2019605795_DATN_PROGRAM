
using Domain.Common;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;
using System;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Security.Claims;
using System.Text;

namespace cm.BackendApi.Helpers
{
    public class TokenHelper : ITokenHelper
    {
        private readonly IConfiguration _configuration;

        public TokenHelper(IConfiguration configuration)
        {
            _configuration = configuration;
        }

        public ClaimsPrincipal GetPrincipal(string token)
        {
            var secrectKey = _configuration["JwtOptions:Secret"];
            var key = Encoding.ASCII.GetBytes(secrectKey);
            try
            {
                var tokenHandler = new JwtSecurityTokenHandler();
                if (!(tokenHandler.ReadToken(token) is JwtSecurityToken jwtToken))
                {
                    return null;
                }

                var validationParameters = new TokenValidationParameters
                {
                    ValidateIssuerSigningKey = true,
                    IssuerSigningKey = new SymmetricSecurityKey(key),
                    ValidateIssuer = false,
                    ValidateAudience = false,
                    ClockSkew = TimeSpan.Zero
                };
                return tokenHandler.ValidateToken(token, validationParameters, out _);
            }
            catch
            {
                return null;
            }
        }

        public Guid GetUserIdToGuid(string accessToken)
        {
            return Guid.NewGuid(); 
        }

        public int GetUserIdToInt(string accessToken)
        {
            var claims = GetPrincipal(accessToken.Replace("Bearer ", string.Empty));
            int userId = 0;
            int.TryParse(claims.Claims.FirstOrDefault(c => c.Type == CommonClaimTypes.UserId)?.Value, out userId);
            return userId;
        }

        public long GetUserIdToLong(string accessToken)
        {
            var claims = GetPrincipal(accessToken.Replace("Bearer ", string.Empty));
            long userId = 0;
            long.TryParse(claims.Claims.FirstOrDefault(c => c.Type == CommonClaimTypes.UserId)?.Value, out userId);
            return userId;
        }

        public string GetUsername(string accessToken)
        {
            var claims = GetPrincipal(accessToken.Replace("Bearer ", string.Empty));
            var res = claims.Claims.FirstOrDefault(c => c.Type == CommonClaimTypes.Name)?.Value;
            return res;
        }
    }
}