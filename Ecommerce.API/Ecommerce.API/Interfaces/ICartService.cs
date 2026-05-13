using Ecommerce.API.DTOs.Cart;

namespace Ecommerce.API.Interfaces
{
    public interface ICartService
    {
        Task AddToCartAsync(int  id,AddToCartDto dto);

        Task<CartResponseDto?> GetCartAsync(int userId);

        Task<bool> RemoveFromCartAsync(
            int userId,
            int productId);

        Task<bool> UpdateQuantityAsync(
            int userId,
            int productId,
            int quantity);

        Task ClearCartAsync(int userId);

    }
}
