using Ecommerce.API.Data;
using Ecommerce.API.DTOs.Cart;
using Ecommerce.API.Entities;
using Ecommerce.API.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace Ecommerce.API.Services
{
    public class CartService : ICartService
    {
        private readonly AppDbContext _context;

        public CartService(AppDbContext context)
        {
            _context = context;
        }
        public async Task AddToCartAsync(int userId, AddToCartDto dto)
        {
            var cart = await _context.Carts
                .Include(x => x.CartItems)
                .FirstOrDefaultAsync(x => x.UserId == userId);

            // Create Cart if not exists
            if (cart == null) 
            {
                cart = new Cart
                {
                    UserId = userId,
                };

                _context.Carts.Add(cart);
                await _context.SaveChangesAsync();
            }

            // Check existing product
            var existingItem = cart.CartItems
                .FirstOrDefault(x => x.ProductId == dto.ProductId);

            if (existingItem != null) 
            {
                existingItem.Quantity += dto.Quantity;
            }
            else
            {
                cart.CartItems.Add(new CartItem
                {
                   ProductId = dto.ProductId,
                   Quantity = dto.Quantity
                });
            }

            await _context.SaveChangesAsync();
        }
        public async Task<CartResponseDto?> GetCartAsync(int userId)
        {
            var cart = await _context.Carts
                .Include(x => x.CartItems)
                .ThenInclude(x => x.Product)
                .FirstOrDefaultAsync(x => x.UserId == userId);

            if (cart == null) 
            {
                return null;
            }

            var response = new CartResponseDto();

            foreach (var item in cart.CartItems) 
            {
                response.Items.Add(new CartItemResponseDto
                {
                    ProductId = item.ProductId,
                    ProductName = item.Product.Name,
                    Quantity = item.Quantity,
                    Price = item.Product.Price,
                    TotalPrice = item.Product.Price * item.Quantity
                });
            }

            response.GrandTotal =
             response.Items.Sum(x => x.TotalPrice);

            return response;
        }

        public async Task<bool> RemoveFromCartAsync(int userId, int productId)
        {
            var cart = await _context.Carts
                .Include(x => x.CartItems)
                .FirstOrDefaultAsync(x => x.UserId == userId);

            if (cart == null) 
            {
                return false;
            }

            var item = cart.CartItems
                .FirstOrDefault(x => x.ProductId == productId);

            if (item == null)
            {
                return false;
            }

            _context.CartItems.Remove(item);
            await _context.SaveChangesAsync();
            return true;
        }

        public async Task<bool> UpdateQuantityAsync(int userId, int productId, int quantity)
        {
            var cart = await _context.Carts
                .Include (x => x.CartItems)
                .FirstOrDefaultAsync (x => x.UserId == userId);

            if (cart == null)
            {
                return false;
            }

            var item = cart.CartItems
                .FirstOrDefault( x => x.ProductId == productId);

            if (item == null)
            {
                return false;
            }

            item.Quantity = quantity;
            await _context.SaveChangesAsync();

            return true;
        }

        public async Task ClearCartAsync(int userId)
        {
            var cart = await _context.Carts
                .Include (x => x.CartItems)
                .FirstOrDefaultAsync(x => x.UserId == userId);

            if (cart == null)
            {
                return;
            }

            _context.CartItems.RemoveRange(cart.CartItems);
            await _context.SaveChangesAsync();
        }
    }
}
