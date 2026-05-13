namespace Ecommerce.API.DTOs.Order
{
    public class OrderResponseDto
    {
        public int OrderId { get; set; }
        public decimal TotalAmount { get; set; }
        public string Status { get; set; } = string.Empty;
        public DateTime CreatedAt { get; set; }

        public List<OrderItemResponseDto> Items { get; set; } = [];
    }
}
