using Ecommerce.API.DTOs.Order;

namespace Ecommerce.API.Interfaces
{
    public interface IOrderService
    {
        Task<OrderResponseDto?> PlaceOrderAsync(int userId);
        Task<List<OrderResponseDto>> getOrderAsync(int userId);

        Task<bool> CancelOrderAsync(int userId,int orderId);
    }
}
