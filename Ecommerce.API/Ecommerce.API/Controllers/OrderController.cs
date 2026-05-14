using Ecommerce.API.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Identity.Client;
using System.Security.Claims;

namespace Ecommerce.API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    [Authorize]
    public class OrderController : ControllerBase
    {
        private readonly IOrderService _orderService;

        public OrderController(IOrderService orderService)
        {
            _orderService = orderService;
        }


        // Order place Api
        [HttpPost]
        public async Task<IActionResult> PlaceOrder()
        {
            int userId = int.Parse(
                User.FindFirst(ClaimTypes.NameIdentifier)!
                .Value);

            var order = await _orderService.PlaceOrderAsync(userId);

            if (order == null)
            {
                return BadRequest("Cart is Empty");
            }

            return Ok(order);
        }

        // Api to get orders
        [HttpGet]
        public async Task<IActionResult> GetOrders()
        {
            int userId = int.Parse(
                User.FindFirst(ClaimTypes.NameIdentifier)!
                .Value);

            var orders = await _orderService.getOrderAsync(userId);
            
            return Ok(orders);
        }

        // Api to cancel order
        [HttpPut("{orderId}/cancel")]
        public async Task<IActionResult> CancelOrder(int orderId)
        {
            int userId = int.Parse(
                User.FindFirst(ClaimTypes.NameIdentifier)!
                .Value);

            bool cancelled = await _orderService.CancelOrderAsync(userId,orderId);

            if(!cancelled)
            {
                return BadRequest("Order can not be cancelled");
            }

            return Ok("Order cancelled successfully");
        }
    }
}
