using InfoGatherer.api.Data.Entities;
using InfoGatherer.api.DTOs.Users;

namespace InfoGatherer.api.Data.Repositories.Interfaces
{
    public interface IRefreshTokenRepository
    {
        Task SaveRefreshTokenAsync(RefreshToken refreshToken);
        Task<RefreshToken> GetRefreshTokenAsync(string token);
        Task UpdateRefreshTokenAsync(RefreshToken refreshToken);
        Task RemoveRefreshTokenAsync(RefreshToken refreshToken);
        Task ClearExpiredRefreshTokensAsync();
    }
}
