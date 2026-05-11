using Ecommerce.API.Data;
using Ecommerce.API.DTOs.Auth;
using Ecommerce.API.Entities;
using Ecommerce.API.Interfaces;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

namespace Ecommerce.API.Services
{
    public class AuthService : IAuthService
    {
        private readonly AppDbContext _context;
        private readonly IConfiguration _configuration;

        public AuthService(AppDbContext context,IConfiguration configuration)
        {
            _context = context;
            _configuration = configuration;
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

            // Generate JWT Token
            string token = GenerateJwtToken(user);

            return new AuthResponseDto
            {
                Token = token
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

            // Generate token
            string token = GenerateJwtToken(user);  

            return new AuthResponseDto
            {
                Token = token
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

    }
}
