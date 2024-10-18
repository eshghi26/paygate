using Common.Helper.Helper;
using Logging.NLog;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

namespace PaymentGateway.Api.Helper
{
    public interface ITokenService
    {
        JwtTokenModel? CreateToken(int employeeId, string userName);
    }

    public class TokenService(IConfiguration configuration, ILoggerManager logger) : ITokenService
    {
        public JwtTokenModel? CreateToken(int employeeId, string userName)
        {
            try
            {
                var claims = new List<Claim>
                {
                    new(JwtRegisteredClaimNames.Jti, SecurityHelper.CreateCryptographicallySecureGuid().ToString(),
                        ClaimValueTypes.String, configuration["Jwt:Issuer"]),
                    new(JwtRegisteredClaimNames.Iss, configuration["Jwt:Issuer"] ?? string.Empty,
                        ClaimValueTypes.String, configuration["Jwt:Issuer"]),
                    new(JwtRegisteredClaimNames.Iat, DateTimeOffset.UtcNow.ToUnixTimeSeconds().ToString(),
                        ClaimValueTypes.Integer64, configuration["Jwt:Issuer"]),
                    new(ClaimTypes.NameIdentifier, employeeId.ToString(), ClaimValueTypes.Integer64,
                        configuration["Jwt:Issuer"]),
                    new(ClaimTypes.Name, userName, ClaimValueTypes.String, configuration["Jwt:Issuer"]),
                };

                var jwtToken = GenerateTokenString(claims);

                if (string.IsNullOrEmpty(jwtToken))
                {
                    return null;
                }

                var refreshTokenSerial = SecurityHelper.CreateCryptographicallySecureGuid().ToString();

                var refreshTokenExpirationMinutes = configuration["Jwt:RefreshTokenExpirationMinutes"];

                if (!int.TryParse(refreshTokenExpirationMinutes, out var expMin))
                {
                    expMin = 60;
                }

                return new JwtTokenModel
                {
                    AccessToken = jwtToken,
                    RefreshTokenSerial = refreshTokenSerial,
                    RefreshTokenExpiryDate = DateTime.UtcNow.AddMinutes(expMin),
                    Claims = claims
                };
            }
            catch (Exception exp)
            {
                logger.LogError($"{exp.Message}{Environment.NewLine}{exp.StackTrace}");
                return null;
            }
        }

        private string? GenerateTokenString(List<Claim> claims)
        {
            try
            {
                var staticKey = configuration["Jwt:Key"];

                if (staticKey == null)
                {
                    return null;
                }

                var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(staticKey));
                var signingCred = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);
                var now = DateTime.UtcNow;
                var expirationMinutes = configuration["Jwt:AccessTokenExpirationMinutes"];

                if (!int.TryParse(expirationMinutes, out var expMin))
                {
                    expMin = 15;
                }

                var securityToken = new JwtSecurityToken(
                    issuer: configuration["Jwt:Issuer"],
                    audience: configuration["Jwt:Audience"],
                    claims: claims,
                    notBefore: now,
                    expires: now.AddMinutes(expMin),
                    signingCredentials: signingCred);

                return new JwtSecurityTokenHandler().WriteToken(securityToken);
            }
            catch (Exception exp)
            {
                logger.LogError($"{exp.Message}{Environment.NewLine}{exp.StackTrace}");
                return null;
            }
        }
    }

    public class JwtTokenModel
    {
        public required string AccessToken { get; set; }
        public required string RefreshTokenSerial { get; set; }
        public required DateTime RefreshTokenExpiryDate { get; set; }
        public List<Claim>? Claims { get; set; }
    }
}
