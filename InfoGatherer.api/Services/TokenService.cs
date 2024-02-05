using InfoGatherer.api.Configuration;
using InfoGatherer.api.Data.Entities;
using InfoGatherer.api.Data.Repositories.Interfaces;
using InfoGatherer.api.DTOs.Users;
using InfoGatherer.api.Services.Interfaces;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Security.Cryptography;
using System.Text;

namespace InfoGatherer.api.Services
{
    public class TokenService : ITokenService
    {
        private readonly JwtConfig _jwtConfig;
        private readonly UserManager<AppUser> _userManager;
        private readonly IRefreshTokenRepository _refreshTokenRepository;
        public TokenService(IOptions<JwtConfig> optionsJwt, UserManager<AppUser> userManager, IRefreshTokenRepository refreshTokenRepository)
        {
            _jwtConfig = optionsJwt.Value!;
            _userManager = userManager;
            _refreshTokenRepository = refreshTokenRepository;
        }
        public async Task<AuthResponseDto> GenerateJwtToken(AppUser user)
        {
            var claims = new List<Claim>
        {
            new Claim(JwtRegisteredClaimNames.Sub, user.UserName),
            new Claim(JwtRegisteredClaimNames.Email, user.Email),
            new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString()),
            new Claim("Id", user.Id.ToString()),
            new Claim("ApiKey", user.ApiKey.ToString())
        };
            var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_jwtConfig.Secret));

            var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha512Signature);

            var tokenDescriptor = new SecurityTokenDescriptor
            {
                Subject = new ClaimsIdentity(claims),
                IssuedAt = DateTime.UtcNow,
                Expires = DateTime.Now.AddSeconds(_jwtConfig.TokenValidityPeriodInSeconds),
                SigningCredentials = creds,
                Issuer = _jwtConfig.TokenIssuer
            };

            var tokenHandler = new JwtSecurityTokenHandler();
            var token = tokenHandler.CreateToken(tokenDescriptor);
            var jwtToken = tokenHandler.WriteToken(token);
            var rTokent = GenerateRefreshToken();
            var refreshToken = new RefreshToken()
            {
                JwtId = token.Id,
                UserId = user.Id,
                AddedDate = DateTime.UtcNow,
                ExpirationDate = DateTime.UtcNow.AddSeconds(_jwtConfig.RefreshTokenValidityPeriodInSeconds),
                Token = rTokent
            };
            await _refreshTokenRepository.SaveRefreshTokenAsync(refreshToken);

            var roles = await _userManager.GetRolesAsync(user);

            return new AuthResponseDto
            {
                Token = jwtToken,
                RefreshToken = refreshToken.Token,
                User = new
                {
                    id = user.Id,
                    email = user.Email,
                    firstname = user.FirstName,
                    lastname = user.LastName,
                    roles
                }
            };
        }
        public async Task<AuthResponseDto> RefreshTokenAsync(string token, string refreshToken)
        {
            var validatedToken = GetPrincipalFromToken(token);
            if (validatedToken == null)
            {
                return null;
            }


            var storedRefreshToken = await _refreshTokenRepository.GetRefreshTokenAsync(refreshToken);
            if (storedRefreshToken == null || DateTime.UtcNow > storedRefreshToken.ExpirationDate)
            {
                return null;
            }

            if (storedRefreshToken.JwtId != validatedToken.Claims.Single(x => x.Type == JwtRegisteredClaimNames.Jti).Value)
            {
                return null;
            }
            //await _refreshTokenRepository.RemoveRefreshTokenAsync(storedRefreshToken);
            var userIdClaim = validatedToken.Claims.FirstOrDefault(x => x.Type == "Id")?.Value;
            var user = await _userManager.FindByIdAsync(userIdClaim);
            return await GenerateJwtToken(user);
        }
        private ClaimsPrincipal GetPrincipalFromToken(string token)
        {
            var tokenHandler = new JwtSecurityTokenHandler();

            try
            {
                var principal = tokenHandler.ValidateToken(token, new TokenValidationParameters
                {
                    ValidateIssuerSigningKey = true,
                    IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_jwtConfig.Secret)),
                    ValidateIssuer = false,
                    ValidateAudience = false,
                    ValidateLifetime = false,
                    ClockSkew = TimeSpan.Zero
                }, out var validatedToken);

                if (!IsJwtWithValidSecurityAlgorithm(validatedToken))
                {
                    return null;
                }

                return principal;
            }
            catch
            {
                return null;
            }
        }
        private static string GenerateRefreshToken()
        {
            var randomNumber = new byte[32];
            using var generator = RandomNumberGenerator.Create();
            generator.GetBytes(randomNumber);
            return Convert.ToBase64String(randomNumber);
        }
        private bool IsJwtWithValidSecurityAlgorithm(SecurityToken validatedToken)
        {
            if (validatedToken is JwtSecurityToken jwtSecurityToken)
            {
                var isValidAlgorithm = jwtSecurityToken.Header.Alg.Equals(SecurityAlgorithms.HmacSha512, StringComparison.InvariantCultureIgnoreCase);

                return isValidAlgorithm;
            }
            return false;
        }
    }
}
