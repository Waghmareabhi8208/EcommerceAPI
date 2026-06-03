using Ecommerce.API.DTOs.Auth;
using Ecommerce.API.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.RateLimiting;
using System.Security.Claims;

namespace Ecommerce.API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class AuthController : ControllerBase
    {
        private readonly IAuthService _authService;

        public AuthController(IAuthService authService)
        {
            _authService = authService;
        }

        [EnableRateLimiting("RegisterPolicy")]
        [HttpPost("register")]
        public async Task<IActionResult> Register(RegisterDto dto)
        {
            var result = await _authService.RegisterAsync(dto);

            if (result == null)
            {
                return BadRequest("Email already existed");
            }

            return Ok(result);
        }

        [EnableRateLimiting("LoginPolicy")]
        [HttpPost("login")]
        public async Task<IActionResult> Login(LoginDto dto)
        {
            var result = await _authService.LoginAsync(dto);

            if (result == null)
                return Unauthorized("Invalid Email or password");

            return Ok(result);
        }

        [HttpGet("me")]
        [Authorize]
        public IActionResult GetCurrentUser()
        {
            var userId =
                User.FindFirst(ClaimTypes.NameIdentifier)?.Value;

            var email =
                User.FindFirst(ClaimTypes.Email)?.Value;

            var role =
                User.FindFirst(ClaimTypes.Role)?.Value;

            return Ok(new
            {
                UserId = userId,
                Email = email,
                Role = role
            });
        }

        // Api Endpoint to add refresh token
        [EnableRateLimiting("RefreshTokenPolicy")]
        [HttpPost("refresh")]
        public async Task<IActionResult> RefreshToken(RefreshTokenRequestDto dto)
        {
            var response = await _authService.RefreshTokenAsync(dto.RefreshToken);

            if(response == null)
            {
                return Unauthorized("Invalid refresh token");
            }

            return Ok(response);
        }

        //Api endpoint for Logout 
        [HttpPost("logout")]
        public async Task<IActionResult> Logout(RefreshTokenRequestDto dto)
        {
            bool loggedOut = await _authService.LogoutAsync(dto.RefreshToken);

            if(!loggedOut)
            {
                return Unauthorized(new
                {
                    message = "Refresh token is invalid or expired"
                });
            }

            return Ok(new
            {
                message = "User logged out successfully"
            });
        }


    }
}
