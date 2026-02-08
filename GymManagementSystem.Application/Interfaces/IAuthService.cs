using GymManagementSystem.Application.DTOs.Auth;

namespace GymManagementSystem.Application.Interfaces
{
    public interface IAuthService
    {
        Task<AuthResponseDto> RegisterAsync(RegisterDto request);

        Task<AuthResponseDto> LoginAsync(LoginDto request);
    }
}