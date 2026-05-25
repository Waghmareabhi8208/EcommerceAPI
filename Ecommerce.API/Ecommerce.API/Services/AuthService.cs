using Ecommerce.API.Data;
using Ecommerce.API.DTOs.Auth;
using Ecommerce.API.Entities;
using Ecommerce.API.Interfaces;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using StackExchange.Redis;
using System.IdentityModel.Tokens.Jwt;
using System.Runtime.InteropServices;
using System.Security.Claims;
using System.Text;

namespace Ecommerce.API.Services
{
    public class AuthService : IAuthService
    {
        private readonly AppDbContext _context;
        private readonly IConfiguration _configuration;
        private readonly IConnectionMultiplexer _redis;

        public AuthService(
            AppDbContext context,
            IConfiguration configuration,
            IConnectionMultiplexer redis)
        {
            _context = context;
            _configuration = configuration;
            _redis = redis;
        }

        public async Task<AuthResponseDto> RegisterAsync(RegisterDto dto)
        {
            // Check if email already exists
            var existingUSer = await _context.Users.FirstOrDefaultAsync(x => x.Email == dto.Email);

            if (existingUSer != null) 
            {
                return null;
            }

            // Hash Password
            string passwordHash = 
                BCrypt.Net.BCrypt.HashPassword(dto.Password);

            // Create User
            var user = new User
            {
                Name = dto.Name,
                Email = dto.Email,
                PasswordHash = passwordHash,
            };

            _context.Users.Add(user);

            await _context.SaveChangesAsync();

            // Generate JWT access Token
            string accesstoken = GenerateJwtToken(user);

            // Generate refresh Token
            string refreshToken = GenerateRefreshToken();

            // Redis database
            var db = _redis.GetDatabase();

            // Store refresh token in Redis
            await db.StringSetAsync(
                $"refresh:{refreshToken}",
                user.Id,
                TimeSpan.FromDays(7));

            return new AuthResponseDto
            {
                AccessToken = accesstoken,
                RefreshToken = refreshToken
            };
        }

        public async Task<AuthResponseDto?> LoginAsync(LoginDto dto)
        {   
            // Find user by Email
            var user = await _context.Users
                .FirstOrDefaultAsync(x => x.Email == dto.Email);

            if (user == null)
            {
                return null;
            }

            bool isPasswordValid =
                BCrypt.Net.BCrypt.Verify(dto.Password,user.PasswordHash);

            if (!isPasswordValid)
            {
                return null;
            }

            // Generate JWT access token
            string accessToken = GenerateJwtToken(user);

            // Generate refresh token
            string refreshToken = GenerateRefreshToken();

            // redis database
            var db = _redis.GetDatabase();

            // Store refresh token in redis
            await db.StringSetAsync(
                $"refresh:{refreshToken}",
                user.Id,
                TimeSpan.FromDays(7));

            return new AuthResponseDto
            {
                AccessToken = accessToken,
                RefreshToken = refreshToken
            };
        }
        
        private string GenerateJwtToken(User user)
        {
            var claims = new[]
            {
                new Claim(ClaimTypes.NameIdentifier, user.Id.ToString()),
                new Claim(ClaimTypes.Email, user.Email),
                new Claim(ClaimTypes.Role,user.Role)
            };

            var key = new SymmetricSecurityKey(
                Encoding.UTF8.GetBytes(
                    _configuration["Jwt:Key"]!));

            var credentials = new SigningCredentials(
                key,
                SecurityAlgorithms.HmacSha256);

            var token = new JwtSecurityToken(
                issuer: _configuration["Jwt:Issuer"],
                audience: _configuration["Jwt:Audience"],
                claims:claims,
                expires:DateTime.UtcNow.AddHours(2),
                signingCredentials:credentials);

            return new JwtSecurityTokenHandler()
                .WriteToken(token);
        }

        // Method to generate RefreshToken
        private string GenerateRefreshToken()
        {
            return Guid.NewGuid().ToString();
        }

        public async Task<AuthResponseDto?> RefreshTokenAsync(string refreshToken)
        {
           var db = _redis.GetDatabase();

            // Check token in redis
            var userIdValue = await db.StringGetAsync(
                $"refresh:{refreshToken}");

            if(userIdValue.IsNullOrEmpty)
            {
                return null;
            }

            int userId = int.Parse(userIdValue!);

            // Get user from database
            var user = await _context.Users
                .FirstOrDefaultAsync(x => x.Id == userId);

            if (user == null) 
            {
                return null;
            }

            // Generate new access Token
            string newAccessToken = GenerateJwtToken(user);

            return new AuthResponseDto
            {
                AccessToken = newAccessToken,

                RefreshToken = refreshToken
            };
        }

        public async Task<bool> LogoutAsync(string refreshToken)
        {
            var db = _redis.GetDatabase();
            
            bool deleted = await db.KeyDeleteAsync(
                $"refresh:{refreshToken}");

            return deleted;
        }
    }
}
