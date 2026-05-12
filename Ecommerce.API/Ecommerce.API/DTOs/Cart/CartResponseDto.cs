namespace Ecommerce.API.DTOs.Cart
{
    public class CartResponseDto
    {
        public List<CartItemResponseDto> Items { get; set; } = [];
        public decimal GrandTotal { get; set; }
    }
}
