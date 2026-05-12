using Ecommerce.API.DTOs.Cart;

namespace Ecommerce.API.Interfaces
{
    public interface ICartService
    {
        Task AddToCartAsync(int  id,AddToCartDto dto);

        Task<CartResponseDto?> GetCartAsync(int userId);
    }
}
