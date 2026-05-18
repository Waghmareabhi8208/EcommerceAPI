using Ecommerce.API.DTOs.Cart;
using Ecommerce.API.Interfaces;
using Ecommerce.API.Services;
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

        // Remove Product Api
        [HttpDelete("{productId}")]
        public async Task<IActionResult> RemoveFromCart(int productId)
        {
            int userId = int.Parse(
                User.FindFirst(ClaimTypes.NameIdentifier)!
                .Value);

            bool removed = await _cartService.RemoveFromCartAsync(userId, productId);

            if(!removed)
            {
                return NotFound("Product not found in cart");
            }

            return NoContent();
        }


        // Update Quantity Api
        [HttpPut]
        public async Task<IActionResult> UpdateQuantity(UpdateCartQuantityDto dto)
        {
            int userId = int.Parse(
                User.FindFirst(ClaimTypes.NameIdentifier)!
                .Value);

            bool updated = await _cartService.UpdateQuantityAsync(userId, dto.ProductId,dto.Quantity);

            if (!updated)
            {
                return NotFound("Product not found in cart");
            }

            return Ok("Cart Updated");
        }

        // Clear Cart Api
        [HttpDelete]
        public async Task<IActionResult> ClearCart()
        {
            int userId = int.Parse(
                User.FindFirst(ClaimTypes.NameIdentifier)!
                .Value);

            await _cartService.ClearCartAsync(userId);

            return NoContent();
        }

        [HttpPost]
        public async Task<IActionResult> PlaceOrder(int addressId)
        {
            int userId = int.Parse(
                User.FindFirst(ClaimTypes.NameIdentifier)!
                    .Value);

            var order =
                await _orderService.PlaceOrderAsync(
                    userId,
                    addressId);

            if (order == null)
            {
                return BadRequest(
                    "Cart is empty or invalid address");
            }

            return Ok(order);
        }
    }
}
