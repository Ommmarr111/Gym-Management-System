using GymManagementSystem.Application.Interfaces;
using GymManagementSystem.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;

namespace GymManagementSystem.Infrastructure.Repositories
{
    public class RefreshTokenRepository : IRefreshTokenRepository
    {
        private readonly ApplicationDbContext _context;

        public RefreshTokenRepository(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<RefreshToken> AddAsync(
            RefreshToken refreshToken)
        {
            await _context.RefreshTokens.AddAsync(refreshToken);

            return refreshToken;
        }

        public async Task<RefreshToken?> GetByTokenHashAsync(
            string tokenHash)
        {
            return await _context.RefreshTokens
                .FirstOrDefaultAsync(
                    rt => rt.TokenHash == tokenHash);
        }

        public Task UpdateAsync(RefreshToken refreshToken)
        {
            _context.RefreshTokens.Update(refreshToken);

            return Task.CompletedTask;
        }
    }
}