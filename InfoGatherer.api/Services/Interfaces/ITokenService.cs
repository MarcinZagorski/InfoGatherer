using InfoGatherer.api.Data.Entities;
using InfoGatherer.api.DTOs.Users;

namespace InfoGatherer.api.Services.Interfaces
{
    public interface ITokenService
    {
        Task<AuthResponseDto> GenerateJwtToken(AppUser user);
        Task<AuthResponseDto> RefreshTokenAsync(string token, string refreshToken);
    }
}
