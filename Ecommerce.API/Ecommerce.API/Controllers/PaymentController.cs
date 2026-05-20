using Ecommerce.API.DTOs.Payment;
using Ecommerce.API.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace Ecommerce.API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    [Authorize]
    public class PaymentController : ControllerBase
    {
        private readonly IPaymentService _paymentService;

        public PaymentController(IPaymentService paymentService)
        {
            _paymentService = paymentService;
        }

        [HttpPost("Create-order")]
        public async Task<IActionResult> CreatePaymentOrder(CreatePaymentDto dto)
        {
            int userId = int.Parse(
                User.FindFirst(
                    ClaimTypes.NameIdentifier)!
                    .Value );

            var response = await _paymentService
                .CreatePaymentOrderAsync(userId, dto.OrderId);

            if (response == null)
            {
                return BadRequest("Invalid order");
            }

            return Ok(response);
        }
    }
}
