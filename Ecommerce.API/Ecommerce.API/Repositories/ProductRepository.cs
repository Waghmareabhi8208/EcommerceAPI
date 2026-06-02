using Ecommerce.API.Data;
using Ecommerce.API.DTOs.Common;
using Ecommerce.API.Entities;
using Ecommerce.API.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace Ecommerce.API.Repositories
{
    public class ProductRepository : IProductRepository
    {
        private readonly AppDbContext _context;

        public ProductRepository(AppDbContext context)
        {
            _context = context;
        }
        public async Task<List<Product>> GetAllAsync(ProductQueryParams queryParams)
        {   
            var query = _context.Products.AsQueryable();

            // Search 
            if(!string.IsNullOrEmpty(
                    queryParams.Search))
            {
                query = query.Where(x =>
                x.Name.Contains(
                    queryParams.Search));
            }

            // Min price
            if(queryParams.MinPrice.HasValue)
            {
                query = query.Where(x => 
                    x.Price >= 
                    queryParams.MinPrice.Value);
            }

            // Max Price
            if (queryParams.MaxPrice.HasValue)
            {
                query = query.Where(x =>
                    x.Price <=
                    queryParams.MaxPrice.Value);
            }

            // In Stock filter
            if (queryParams.InStock.HasValue)
            {
                query = queryParams.InStock.Value
                    ? query.Where(x => x.Stock > 0)

                    : query.Where(x => x.Stock <= 0);
            }

            // sorting
            query = queryParams.SortBy switch
            {
                "price_asc" =>
                    query.OrderBy(x => x.Price),

                "price_desc" =>
                    query.OrderByDescending(
                        x => x.Price),

                "name" =>
                    query.OrderBy(x => x.Name),

                _ =>
                    query.OrderBy(x => x.Id)
            };

            return await query
                .Skip(
                (queryParams.PageNumber - 1)
                * queryParams.PageSize)
                
                .Take(queryParams.PageSize)
                .ToListAsync();
        }
        public async Task<Product> AddAsync(Product product)
        {
            _context.Products.Add(product);
            await _context.SaveChangesAsync();
            return product;
        }

        public async Task<Product?> GetByIdAsync(int id)
        {
            return await _context.Products.FindAsync(id);
        }

        public async Task<Product?> UpdateAsync(int id, Product product)
        {
            var existingProduct = await _context.Products.FindAsync(id);

            if (existingProduct == null)
            {
                return null;
            }

            existingProduct.Name = product.Name;
            existingProduct.Description = product.Description;
            existingProduct.Price = product.Price;
            existingProduct.Stock = product.Stock;
            existingProduct.ImageUrl = product.ImageUrl;


            await _context.SaveChangesAsync();
            return existingProduct;
        }

        public async Task<bool> DeleteAsync(int id)
        {
            var product = await _context.Products.FindAsync(id);

            if (product == null) 
                return false;

            _context.Products.Remove(product);

            await _context.SaveChangesAsync();

            return true;
        }
    }
}
