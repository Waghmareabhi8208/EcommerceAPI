namespace Ecommerce.API.DTOs.Cart
{
    public class UpdateCartQuantityDto
    {
        public int ProductId { get; set; }
        public int Quantity { get; set; }
    }
}
