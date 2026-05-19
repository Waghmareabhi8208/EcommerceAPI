using Ecommerce.API.Data;
using Ecommerce.API.DTOs.Payment;
using Ecommerce.API.Interfaces;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.ChangeTracking;
using Razorpay.Api;

namespace Ecommerce.API.Services
{
    public class PaymentService : IPaymentService
    {
        private readonly AppDbContext _context;
        private readonly IConfiguration _configuration;

        public PaymentService(AppDbContext context,IConfiguration configuration)
        {
            _context = context;
            _configuration = configuration;
        }
        public async Task<PaymentOrderResponseDto?> CreatePaymentOrderAsync(int userId, int orderId)
        {
            var order = await _context.Orders
                .FirstOrDefaultAsync(x => x.Id == orderId && x.UserId == userId);

            if (order == null)
            {
                return null;
            }

            // Razorpay credentials
            string key = _configuration["Razorpay:Key"]!;
            string secret = _configuration["Razorpay:Secret"]!;
            RazorpayClient client = new RazorpayClient(key, secret);

            // Amount in paise
            int amount = (int)(order.TotalAmount * 100);

            Dictionary<string, object> options = new Dictionary<string, object>
            {
                {"amount",amount},
                {"currency","INR"},
                {"receipt",$"order_{order.Id}" }
            };

            Razorpay.Api.Order razorpayOrder = client.Order.Create(options);

            string razorpayOrderId =
              razorpayOrder["id"].ToString();

            // Save Razorpay order id
            order.RazorpayOrderId =
                razorpayOrderId;

            await _context.SaveChangesAsync();

            return new PaymentOrderResponseDto
            {
                RazorpayOrderId = razorpayOrderId,

                Amount = order.TotalAmount,

                Currency = "INR",

                Key = key
            };
        }
    }
}
