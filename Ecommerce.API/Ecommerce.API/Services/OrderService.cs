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
        private readonly ILogger<OrderService> _logger;
        public OrderService(AppDbContext context, ILogger<OrderService> logger)
        {
            _context = context;
            _logger = logger;
        }
        public async Task<OrderResponseDto?> PlaceOrderAsync(int userId,int addressId)
        {
            var cart = await _context.Carts
                .Include(x => x.CartItems)
                .ThenInclude(x => x.Product)
                .FirstOrDefaultAsync(x => x.UserId == userId);

            if (cart == null || !cart.CartItems.Any())
            {
                _logger.LogWarning(
                   "Order placement failed. Cart is empty for UserId: {UserId}",
                   userId);

                return null;
            }

            // Get selected address
            var address = await _context.Addresses
                .FirstOrDefaultAsync(x =>
                    x.Id == addressId &&
                    x.UserId == userId);

            // Validate address ownership
            if (address == null)
            {
                _logger.LogWarning(
                       "Order placement failed. Invalid address selected. UserId: {UserId}, AddressId: {AddressId}",
                       userId,
                       addressId);

                return null;
            }

            var order = new Order
            {
                UserId = userId,

                ShippingFullName = address.FullName,

                ShippingPhoneNumber = address.PhoneNumber,

                ShippingStreet = address.Street,

                ShippingCity = address.City,

                ShippingState = address.State,

                ShippingPostalCode = address.PostalCode,

                ShippingCountry = address.Country
            };

            decimal total = 0;

            foreach (var item in cart.CartItems)
            {   
                // Stock Validation
                if (item.Quantity > item.Product.Stock)
                {
                    _logger.LogWarning(
                        "Order placement failed due to insufficient stock. ProductId: {ProductId}, ProductName: {ProductName}, AvailableStock: {Stock}, RequestedQuantity: {Quantity}",
                        item.Product.Id,
                        item.Product.Name,
                        item.Product.Stock,
                        item.Quantity);

                    throw new Exception(
                        $"Only {item.Product.Stock} items available for {item.Product.Name}");
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

            _logger.LogInformation(
                   "Order placed successfully. OrderId: {OrderId}, UserId: {UserId}, TotalAmount: {TotalAmount}",
                   order.Id,
                   userId,
                   order.TotalAmount);

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
                .Include(x => x.User)
                .Include(x => x.OrderItems)
                .ThenInclude(x => x.Product)
                .Where(x => x.UserId == userId)
                .ToListAsync();

            _logger.LogInformation(
               "Customer orders retrieved successfully. UserId: {UserId}, TotalOrders: {TotalOrders}",
               userId,
               orders.Count);

            return orders.Select(order =>
               new OrderResponseDto
               {
                   OrderId = order.Id,

                   TotalAmount = order.TotalAmount,

                   Status = order.Status,

                   CreatedAt = order.CreatedAt,

                   CustomerName = order.User.Name,

                   CustomerEmail = order.User.Email,

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

        public async Task<bool> CancelOrderAsync(int userId, int orderId)
        {
            var order = await _context.Orders
                .Include(x => x.OrderItems)
                .ThenInclude(x => x.Product)
                .FirstOrDefaultAsync(x => x.Id == orderId && x.UserId == userId);

            if (order == null)
            {
                _logger.LogWarning(
                    "Order cancellation failed. Order not found. OrderId: {OrderId}, UserId: {UserId}",
                    orderId,
                    userId);

                return false;
            }
                

            // Prevent invalid Cancellation
            if(order.Status == "Delivered" ||  order.Status == "Cancelled")
            {
                _logger.LogWarning(
                      "Order cancellation failed. Invalid order status. OrderId: {OrderId}, CurrentStatus: {Status}",
                      order.Id,
                      order.Status);

                return false;

            }
            

            // Restore Stock
            foreach(var item in order.OrderItems)
            {
                item.Product.Stock += item.Quantity;
            }

            order.Status = "Cancelled";

            await _context.SaveChangesAsync();

            _logger.LogInformation(
                   "Order cancelled successfully. OrderId: {OrderId}, UserId: {UserId}",
                   order.Id,
                   userId);
            
            return true;
        }

        public async Task<bool> UpdateOrderStatusAsync(int orderId, string status)
        {
            var order = await _context.Orders
                .FirstOrDefaultAsync(x => x.Id == orderId);

            if (order == null)
            {
                _logger.LogWarning(
                   "Order status update failed. Order not found. OrderId: {OrderId}",
                   orderId);

                return false;
            }
            

            if(order.Status == status)
            {
                _logger.LogWarning(
                       "Order status update skipped. Order already has status: {Status}. OrderId: {OrderId}",
                       status,
                       orderId);

                return false;
            }

            // Update status
            order.Status = status;

            await _context.SaveChangesAsync();

            _logger.LogInformation(
                   "Order status updated successfully. OrderId: {OrderId}, NewStatus: {Status}",
                   order.Id,
                   status);

            return true;
        }

        public async Task<OrderResponseDto?> GetOrderByIdAsync(int userId, int orderId)
        {
            var order = await _context.Orders
                .Include (x => x.OrderItems)
                .ThenInclude(x => x.Product)
                .FirstOrDefaultAsync (x => x.Id == orderId && x.UserId == userId);

            if (order == null)
            {
                _logger.LogWarning(
                    "Order retrieval failed. Order not found. OrderId: {OrderId}, UserId: {UserId}",
                    orderId,
                    userId);

                return null;
            }

            _logger.LogInformation(
                "Order retrieved successfully. OrderId: {OrderId}, UserId: {UserId}",
                order.Id,
                userId);

            return new OrderResponseDto
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

                    TotalPrice = item.Price * item.Quantity

                }).ToList()
            };
        }

        public async Task<List<OrderResponseDto>> GetAllOrdersAsync()
        {
            var orders = await _context.Orders
                .Include(x => x.User)
                .Include(x => x.OrderItems)
                .ThenInclude(x => x.Product)
                .ToListAsync();

            _logger.LogInformation(
                 "All orders retrieved successfully. TotalOrders: {TotalOrders}",
                 orders.Count);

            return orders.Select(order =>new OrderResponseDto
           {
               OrderId = order.Id,
               
               CustomerName = order.User.Name,

               CustomerEmail = order.User.Email,

               TotalAmount = order.TotalAmount,

               Status = order.Status,

               CreatedAt = order.CreatedAt,

               Items = order.OrderItems.Select(item =>
                   new OrderItemResponseDto
                   {
                       ProductName = item.Product.Name,

                       Quantity = item.Quantity,

                       Price = item.Price,

                       TotalPrice = item.Price * item.Quantity
                   }).ToList()
            }).ToList();
        }

        // Admin can see one specific customer order details
        public async Task<OrderResponseDto?> GetAnyOrderByIdAsync(int orderId)
        {
            var order = await _context.Orders
                .Include(x => x.User)
                .Include(x => x.OrderItems)
                .ThenInclude(x => x.Product)
                .FirstOrDefaultAsync(x => x.Id == orderId);

            if (order == null)
            {
                _logger.LogWarning(
                   "Admin order retrieval failed. Order not found. OrderId: {OrderId}",
                   orderId);

                return null;
            }

            _logger.LogInformation(
               "Admin retrieved order successfully. OrderId: {OrderId}, CustomerId: {UserId}",
               order.Id,
               order.UserId);

            return new OrderResponseDto
            {
                OrderId = order.Id,

                CustomerName = order.User.Name,

                CustomerEmail = order.User.Email,

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
            };
        }

        
    }
}
