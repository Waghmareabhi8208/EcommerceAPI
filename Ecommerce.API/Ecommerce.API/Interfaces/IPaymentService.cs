using Ecommerce.API.DTOs.Payment;

namespace Ecommerce.API.Interfaces
{
    public interface IPaymentService
    {
        Task<PaymentOrderResponseDto?> CreatePaymentOrderAsync(int userId, int orderId);
        Task<bool> VerifyPaymentAsync(int userId,VerifyPaymentDto dto);
    }
}
