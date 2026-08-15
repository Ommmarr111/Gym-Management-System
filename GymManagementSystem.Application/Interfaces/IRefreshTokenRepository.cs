namespace GymManagementSystem.Application.Interfaces
{
    public interface IRefreshTokenRepository
    {
        Task<RefreshToken> AddAsync(RefreshToken refreshToken);

        Task<RefreshToken?> GetByTokenHashAsync(string tokenHash);

        Task UpdateAsync(RefreshToken refreshToken);
    }
}