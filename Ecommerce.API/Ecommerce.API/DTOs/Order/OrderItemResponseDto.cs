namespace Ecommerce.API.DTOs.Order
{
    public class OrderItemResponseDto
    {
        public string ProductName { get; set; } = string.Empty;
        public int Quantity { get; set; }
        public decimal Price { get; set; }
        public decimal TotalPrice { get; set; }
    }
}
