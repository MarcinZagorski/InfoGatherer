using InfoGatherer.api.Data.Entities;
using InfoGatherer.api.Data.Repositories.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace InfoGatherer.api.Data.Repositories
{
    public class RefreshTokenRepository : IRefreshTokenRepository
    {
        private readonly AppDbContext _ctx;

        public RefreshTokenRepository(AppDbContext ctx)
        {
            _ctx = ctx;
        }

        public async Task SaveRefreshTokenAsync(RefreshToken refreshToken)
        {
            await _ctx.RefreshTokens.AddAsync(refreshToken);
            await _ctx.SaveChangesAsync();
        }
        public async Task<RefreshToken> GetRefreshTokenAsync(string token)
        {
            return await _ctx.RefreshTokens.FirstOrDefaultAsync(rt => rt.Token == token);
        }

        public async Task UpdateRefreshTokenAsync(RefreshToken refreshToken)
        {
            _ctx.RefreshTokens.Update(refreshToken);
            await _ctx.SaveChangesAsync();
        }
        public async Task RemoveRefreshTokenAsync(RefreshToken refreshToken)
        {
            _ctx.RefreshTokens.Remove(refreshToken);
            await _ctx.SaveChangesAsync();
        }
        public async Task ClearExpiredRefreshTokensAsync()
        {
            var now = DateTime.UtcNow;
            var expiredTokens = _ctx.RefreshTokens.Where(rt => rt.ExpirationDate < now);
            _ctx.RefreshTokens.RemoveRange(expiredTokens);
            await _ctx.SaveChangesAsync();
        }
    }
}
