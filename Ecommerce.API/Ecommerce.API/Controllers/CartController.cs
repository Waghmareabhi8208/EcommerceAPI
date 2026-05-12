using Ecommerce.API.DTOs.Cart;
using Ecommerce.API.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace Ecommerce.API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    [Authorize]
    public class CartController : ControllerBase
    {
        private readonly ICartService _cartService;

        public CartController(ICartService cartService)
        {
            _cartService = cartService;
        }

        [HttpPost]
        public async Task<IActionResult> AddToCart(AddToCartDto dto)
        {
            int userId = int.Parse(
                User.FindFirst(ClaimTypes.NameIdentifier)!.Value);

            await _cartService.AddToCartAsync(userId, dto);

            return Ok("Product added to cart");
        }

        [HttpGet]
        public async Task<IActionResult> GetCart()
        {
            int userId = int.Parse(
                User.FindFirst(ClaimTypes.NameIdentifier)!.Value);

            var cart = await _cartService.GetCartAsync(userId);

            if (cart == null)
            {
                return NotFound("Cart is empty");
            }

            return Ok(cart);
        }
    }
}
