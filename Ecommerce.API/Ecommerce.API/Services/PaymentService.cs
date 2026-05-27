using Ecommerce.API.Data;
using Ecommerce.API.DTOs.Payment;
using Ecommerce.API.Interfaces;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.ChangeTracking;
using Razorpay.Api;
using System.Security.Cryptography;
using System.Text;

namespace Ecommerce.API.Services
{
    public class PaymentService : IPaymentService
    {
        private readonly AppDbContext _context;
        private readonly IConfiguration _configuration;
        private readonly ILogger<PaymentService> _logger;
        public PaymentService(
            AppDbContext context,
            IConfiguration configuration,
            ILogger<PaymentService> logger)
        {
            _context = context;
            _configuration = configuration;
            _logger = logger;
        }
        public async Task<PaymentOrderResponseDto?> CreatePaymentOrderAsync(int userId, int orderId)
        {
            var order = await _context.Orders
                .FirstOrDefaultAsync(x => x.Id == orderId && x.UserId == userId);

            // Check order exists
            if (order == null)
            {
                _logger.LogWarning(
                    "Payment order creation failed. Order not found. OrderId: {OrderId}, UserId: {UserId}",
                    orderId,
                    userId);

                return null;
            }

            // Prevent duplicate payment
            if (order.PaymentStatus == "Paid")
            {
                _logger.LogWarning(
                    "Payment order creation failed. Order already paid. OrderId: {OrderId}",
                    order.Id);

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

            _logger.LogInformation(
               "Payment order created successfully. OrderId: {OrderId}, RazorpayOrderId: {RazorpayOrderId}, Amount: {Amount}",
               order.Id,
               razorpayOrderId,
               order.TotalAmount);


            return new PaymentOrderResponseDto
            {
                RazorpayOrderId = razorpayOrderId,

                Amount = order.TotalAmount,

                Currency = "INR",

                Key = key
            };
        }

        // To verify payment is properly done or not because we should not trust frontend
        // it can easily manipulated.
        public async Task<bool> VerifyPaymentAsync(int userId, VerifyPaymentDto dto)
        {
            var order = await _context.Orders
                .FirstOrDefaultAsync(x => x.Id == dto.OrderId
                && x.UserId == userId);

            // Check order exists
            if (order == null)
            {
                _logger.LogWarning(
                    "Payment verification failed. Order not found. OrderId: {OrderId}, UserId: {UserId}",
                    dto.OrderId,
                    userId);

                return false;
            }

            // Prevent duplicate verification
            if (order.PaymentStatus == "Paid")
            {
                _logger.LogWarning(
                    "Payment verification skipped. Order already paid. OrderId: {OrderId}",
                    order.Id);

                return false;
            }

            string secret = _configuration["Razorpay:Secret"]!;

            // Generate expected signature
            string payload = dto.RazorpayOrderId + "|" + dto.RazorpayPaymentId;

            using var hmac = new HMACSHA256(Encoding.UTF8.GetBytes(secret));

            byte[] hash = hmac.ComputeHash(Encoding.UTF8.GetBytes(payload));

            string generatedSignature =
                BitConverter
                .ToString(hash)
                .Replace("-","")
                .ToLower();

            // Compare Signature
            bool isValid = generatedSignature == dto.RazorpaySignature;

            if (!isValid)
            {
                _logger.LogWarning(
                    "Payment verification failed due to invalid signature. OrderId: {OrderId}",
                    dto.OrderId);

                return false;
            }

            // Mark payment success 
            order.PaymentStatus = "Paid";

            order.RazorpayPaymentId = dto.RazorpayPaymentId;

            await _context.SaveChangesAsync();
            
            _logger.LogInformation(
                "Payment verified successfully. OrderId: {OrderId}, PaymentId: {PaymentId}",
                order.Id,
                dto.RazorpayPaymentId);
            
            return true;
        }
    }
}
