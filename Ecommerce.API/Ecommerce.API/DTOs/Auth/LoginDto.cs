using Microsoft.AspNetCore.Mvc.Infrastructure;

namespace Ecommerce.API.DTOs.Auth
{
    public class LoginDto
    {
        public string Email { get; set; } = string.Empty;

        public string Password { get; set; } = string.Empty;
    }
}
