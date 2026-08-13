using GymManagementSystem.Application.DTOs.Auth;
using GymManagementSystem.Application.Exceptions;
using GymManagementSystem.Application.Interfaces;
using GymManagementSystem.Domain.Entities;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.RateLimiting;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

namespace GymManagementSystem.Application.Services
{
    public class AuthService : IAuthService
    {
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly IConfiguration _configuration;

        public AuthService(UserManager<ApplicationUser> userManager, IConfiguration configuration)
        {
            _userManager = userManager;
            _configuration = configuration;
        }

        public async Task<AuthResponseDto> RegisterAsync(RegisterDto dto)
        {
            var existingUser = await _userManager.FindByEmailAsync(dto.Email);

            if (existingUser != null)
                throw new BusinessRuleException($"User with email {dto.Email} already exists");

            var user = new ApplicationUser
            {
                Email = dto.Email,
                UserName = dto.Email,
                FullName = dto.FullName,
                EmailConfirmed = true
            };

            var result = await _userManager.CreateAsync(user, dto.Password);

            if (!result.Succeeded)
            {
                var errors = string.Join(", ", result.Errors.Select(e => e.Description));
                throw new ValidationException($"User creation failed: {errors}");
            }
            await _userManager.AddToRoleAsync(user, dto.Role);
            return await GenerateToken(user);
        }

        [EnableRateLimiting("StrictAuth")]
        public async Task<AuthResponseDto> LoginAsync(LoginDto dto)
        {
            var user = await _userManager.FindByEmailAsync(dto.Email);

            if (user == null)
                throw new UnauthorizedException("Invalid email or password");

            var isPasswordValid = await _userManager.CheckPasswordAsync(user, dto.Password);

            if (!isPasswordValid)
                throw new UnauthorizedException("Invalid email or password");

            return await GenerateToken(user);
        }

        private async Task<AuthResponseDto> GenerateToken(ApplicationUser user)
        {
            var claims = new List<Claim>
            {
                new Claim(ClaimTypes.NameIdentifier, user.Id),
                new Claim(ClaimTypes.Email, user.Email!),
                new Claim("FullName", user.FullName)
            };

            var roles = await _userManager.GetRolesAsync(user);

            foreach (var role in roles)
            {
                claims.Add(new Claim(ClaimTypes.Role, role));
            }

            var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_configuration["Jwt:Key"]!));
            var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

            var tokenDescriptor = new SecurityTokenDescriptor
            {
                Subject = new ClaimsIdentity(claims),
                Expires = DateTime.UtcNow.AddMinutes(double.Parse(_configuration["Jwt:DurationInMinutes"]!)),
                Issuer = _configuration["Jwt:Issuer"],
                Audience = _configuration["Jwt:Audience"],
                SigningCredentials = creds
            };

            var tokenHandler = new JwtSecurityTokenHandler();
            var token = tokenHandler.CreateToken(tokenDescriptor);

            return new AuthResponseDto
            {
                Token = tokenHandler.WriteToken(token),
                UserId = user.Id,
                Email = user.Email!,
                FullName = user.FullName,
                Roles = roles.ToList(),
                Expiration = DateTime.UtcNow.AddMinutes(
                    double.Parse(_configuration["Jwt:DurationInMinutes"]!))
            };
        }
    }
}