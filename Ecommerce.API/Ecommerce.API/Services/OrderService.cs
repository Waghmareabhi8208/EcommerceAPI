using Ecommerce.API.Data;
using Ecommerce.API.DTOs.Order;
using Ecommerce.API.Entities;
using Ecommerce.API.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace Ecommerce.API.Services
{
    public class OrderService : IOrderService
    {
        private readonly AppDbContext _context;

        public OrderService(AppDbContext context)
        {
            _context = context;
        }
        public async Task<OrderResponseDto?> PlaceOrderAsync(int userId)
        {
            var cart = await _context.Carts
                .Include(x => x.CartItems)
                .ThenInclude(x => x.Product)
                .FirstOrDefaultAsync(x => x.UserId == userId);

            if (cart == null || !cart.CartItems.Any())
            {
                return null;
            }

            var order = new Order
            {
                UserId = userId
            };

            decimal total = 0;

            foreach (var item in cart.CartItems)
            {   
                // Stock Validation
                if (item.Quantity > item.Product.Stock)
                {
                    return null;
                }

                // Add order item
                order.OrderItems.Add(new OrderItem
                {
                    ProductId = item.ProductId,
                    Quantity = item.Quantity,
                    // Snapshot price
                    Price = item.Product.Price

                });

                // Reduce stock
                item.Product.Stock -= item.Quantity;
                
                // Calculate total
                total += item.Product.Price * item.Quantity;
            }

            order.TotalAmount = total;
            _context.Orders.Add(order);

            // Clear cart after order
            _context.CartItems.RemoveRange(cart.CartItems);

            await _context.SaveChangesAsync();

            return new OrderResponseDto
            {
                OrderId = order.Id,

                TotalAmount = order.TotalAmount,

                Status = order.Status,

                CreatedAt = order.CreatedAt,

                Items = order.OrderItems.Select(x =>
                    new OrderItemResponseDto
                    {
                        ProductName = x.Product.Name,

                        Quantity = x.Quantity,

                        Price = x.Price,

                        TotalPrice = x.Price * x.Quantity
                    }).ToList()
            };
        }
        public async Task<List<OrderResponseDto>> getOrderAsync(int userId)
        {
            var orders = await _context.Orders
               .Include(x => x.OrderItems)
               .ThenInclude(x => x.Product)
               .Where(x => x.UserId == userId)
               .ToListAsync();

            return orders.Select(order =>
               new OrderResponseDto
               {
                   OrderId = order.Id,

                   TotalAmount = order.TotalAmount,

                   Status = order.Status,

                   CreatedAt = order.CreatedAt,

                   Items = order.OrderItems.Select(item =>
                       new OrderItemResponseDto
                       {
                           ProductName = item.Product.Name,

                           Quantity = item.Quantity,

                           Price = item.Price,

                           TotalPrice =
                               item.Price * item.Quantity
                       }).ToList()
               }).ToList();
        }
    }
}
