using Ecommerce.API.DTOs.Order;
using Ecommerce.API.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
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


        //Api for Get details of particular order
        [HttpGet("{orderId}")]
        public async Task<IActionResult> GetOrderById(int orderId)
        {
            int userId = int.Parse(
                User.FindFirst(ClaimTypes.NameIdentifier)!
                    .Value);

            var order =
                await _orderService.GetOrderByIdAsync(
                    userId,
                    orderId);

            if (order == null)
            {
                return NotFound();
            }

            return Ok(order);
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

        // Admin endpoint to update order status
        [HttpPut("admin/{orderId}/status")]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> UpdateStatus(int orderId,UpdateOrderStatusDto dto)
        {
            bool updated =  await _orderService.UpdateOrderStatusAsync(orderId, dto.Status);

            if(!updated)
            {
                return NotFound();
            }

            return Ok("Order status updated");
        }

        // Api for admin to see customers all order details
        [HttpGet("admin/all")]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> GetAllOrders()
        {
            var orders =
                await _orderService.GetAllOrdersAsync();

            return Ok(orders);
        }

        // Endpoint for admin to get specific customer order details
        [HttpGet("admin/{orderId}")]
        [Authorize(Roles ="Admin")]
        public async Task<IActionResult> GetAnyOrderById(int orderId)
        {
            var order = await _orderService.GetAnyOrderByIdAsync(orderId);

            if(order == null)
                return NotFound();

            return Ok(order);
        }
    }
}
