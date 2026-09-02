using GymManagementSystem.Application.DTOs.Auth;
using GymManagementSystem.Application.Exceptions;
using GymManagementSystem.Application.Interfaces;
using GymManagementSystem.Domain.Entities;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Security.Cryptography;
using System.Text;

namespace GymManagementSystem.Application.Services
{
    public class AuthService : IAuthService
    {
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly IConfiguration _configuration;
        private readonly IUnitOfWork _unitOfWork;

        public AuthService(
            UserManager<ApplicationUser> userManager,
            IConfiguration configuration,
            IUnitOfWork unitOfWork)
        {
            _userManager = userManager;
            _configuration = configuration;
            _unitOfWork = unitOfWork;
        }

        public async Task<AuthResponseDto> RegisterAsync(RegisterDto dto)
        {
            var existingUser = await _userManager.FindByEmailAsync(dto.Email);

            if (existingUser != null)
                throw new BusinessRuleException(
                    $"User with email {dto.Email} already exists");

            var user = new ApplicationUser
            {
                Email = dto.Email,
                UserName = dto.Email,
                FullName = dto.FullName,
                EmailConfirmed = true
            };

            var result = await _userManager.CreateAsync(
                user,
                dto.Password);

            if (!result.Succeeded)
            {
                var errors = string.Join(
                    ", ",
                    result.Errors.Select(e => e.Description));

                throw new ValidationException(
                    $"User creation failed: {errors}");
            }

            await _userManager.AddToRoleAsync(
                user,
                dto.Role);

            return await GenerateAuthTokensAsync(user);
        }

        public async Task<AuthResponseDto> LoginAsync(LoginDto dto)
        {
            var user = await _userManager.FindByEmailAsync(dto.Email);

            if (user == null)
                throw new UnauthorizedException(
                    "Invalid email or password");

            var isPasswordValid =
                await _userManager.CheckPasswordAsync(
                    user,
                    dto.Password);

            if (!isPasswordValid)
                throw new UnauthorizedException(
                    "Invalid email or password");

            return await GenerateAuthTokensAsync(user);
        }

        private async Task<AuthResponseDto> GenerateAuthTokensAsync(ApplicationUser user)
        {
            // Get roles once.
            var roles = await _userManager.GetRolesAsync(user);

            // 1. Generate short-lived JWT Access Token.
            var accessToken = GenerateAccessToken(
                user,
                roles);

            // 2. Generate raw Refresh Token.
            var rawRefreshToken =
                GenerateRandomTokenString();

            // 3. Hash the Refresh Token before storing it.
            var refreshTokenHash =
                HashToken(rawRefreshToken);

            // 4. Create RefreshToken entity.
            var refreshToken = new RefreshToken
            {
                UserId = user.Id,
                TokenHash = refreshTokenHash,
                CreatedOn = DateTime.UtcNow,
                ExpiresOn = DateTime.UtcNow.AddDays(7)
            };

            // 5. Add RefreshToken to the database context.
            await _unitOfWork.RefreshTokens.AddAsync(
                refreshToken);

            // 6. Save changes to the database.
            await _unitOfWork.SaveChangesAsync();

            // 7. Return both tokens to the client.
            return new AuthResponseDto
            {
                AccessToken = accessToken,
                RefreshToken = rawRefreshToken,

                UserId = user.Id,
                Email = user.Email!,
                FullName = user.FullName,
                Roles = roles.ToList(),

                Expiration = DateTime.UtcNow.AddMinutes(
                    double.Parse(
                        _configuration["Jwt:DurationInMinutes"]!))
            };
        }

        private string GenerateAccessToken(ApplicationUser user, IList<string> roles)
        {
            var claims = new List<Claim>
            {
                new Claim(
                    ClaimTypes.NameIdentifier,
                    user.Id),

                new Claim(
                    ClaimTypes.Email,
                    user.Email!),

                new Claim(
                    "FullName",
                    user.FullName)
            };

            foreach (var role in roles)
            {
                claims.Add(
                    new Claim(
                        ClaimTypes.Role,
                        role));
            }

            var key = new SymmetricSecurityKey(
                Encoding.UTF8.GetBytes(
                    _configuration["Jwt:Key"]!));

            var credentials = new SigningCredentials(
                key,
                SecurityAlgorithms.HmacSha256);

            var expiration = DateTime.UtcNow.AddMinutes(
                double.Parse(
                    _configuration["Jwt:DurationInMinutes"]!));

            var tokenDescriptor = new SecurityTokenDescriptor
            {
                Subject = new ClaimsIdentity(claims),

                Expires = expiration,

                Issuer = _configuration["Jwt:Issuer"],

                Audience = _configuration["Jwt:Audience"],

                SigningCredentials = credentials
            };

            var tokenHandler = new JwtSecurityTokenHandler();

            var token = tokenHandler.CreateToken(
                tokenDescriptor);

            return tokenHandler.WriteToken(token);
        }

        private static string GenerateRandomTokenString()
        {
            var randomBytes =
                RandomNumberGenerator.GetBytes(64);

            return Convert.ToBase64String(
                randomBytes);
        }

        private static string HashToken(string token)
        {
            var hash = SHA256.HashData(
                Encoding.UTF8.GetBytes(token));

            return Convert.ToBase64String(
                hash);
        }

        public async Task<AuthResponseDto> RefreshTokenAsync(string refreshToken)
        {
            var tokenHash = HashToken(refreshToken);

            var storedToken = await _unitOfWork.RefreshTokens.GetByTokenHashAsync(tokenHash);

            if (storedToken == null)
                throw new UnauthorizedException("Invalid refresh token");

            bool wasAlreadyRevoked = storedToken.RevokedOn != null;
            bool isExpired = storedToken.ExpiresOn <= DateTime.UtcNow;

            var rowsAffected = await _unitOfWork.RefreshTokens.RevokeIfActiveAsync(storedToken.Id);

            if (rowsAffected == 0)
            {
                if (wasAlreadyRevoked && !isExpired)
                {
                    await _unitOfWork.RefreshTokens.RevokeAllForUserAsync(storedToken.UserId);
                    throw new UnauthorizedException("Token reuse detected. All sessions have been revoked.");
                }
                throw new UnauthorizedException("Refresh token is expired or revoked");
            }

            await using var transaction = await _unitOfWork.BeginTransactionAsync();
            try
            {
                var user = await _userManager.FindByIdAsync(storedToken.UserId);
                if (user == null)
                    throw new UnauthorizedException("User associated with refresh token not found");

                var roles = await _userManager.GetRolesAsync(user);
                var newAccessToken = GenerateAccessToken(user, roles);

                var newRawRefreshToken = GenerateRandomTokenString();
                var newRefreshToken = new RefreshToken
                {
                    UserId = user.Id,
                    TokenHash = HashToken(newRawRefreshToken),
                    CreatedOn = DateTime.UtcNow,
                    ExpiresOn = DateTime.UtcNow.AddDays(7)
                };

                await _unitOfWork.RefreshTokens.AddAsync(newRefreshToken);
                await _unitOfWork.SaveChangesAsync();
                await transaction.CommitAsync();

                return new AuthResponseDto
                {
                    AccessToken = newAccessToken,
                    RefreshToken = newRawRefreshToken,
                    UserId = user.Id,
                    Email = user.Email!,
                    FullName = user.FullName,
                    Roles = roles.ToList(),
                    Expiration = DateTime.UtcNow.AddMinutes(
                        double.Parse(_configuration["Jwt:DurationInMinutes"]!))
                };
            }
            catch
            {
                await transaction.RollbackAsync();
                throw;
            }
        }
    }
}