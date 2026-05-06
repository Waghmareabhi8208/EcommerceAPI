using Ecommerce.API.Data;
using Ecommerce.API.Entities;
using Microsoft.AspNetCore.Mvc;

namespace Ecommerce.API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class ProductController : ControllerBase
    {
        private readonly AppDbContext _context;

        public ProductController(AppDbContext context)
        {
            _context = context;
        }

        // Gives the list of All products
        [HttpGet]
        public IActionResult GetProducts()
        {
            var products = _context.Products.ToList();

            return Ok(products);
        }

        [HttpPost]
        public IActionResult AddProduct(Product product)
        {
            _context.Products.Add(product);

            _context.SaveChanges();

            return Ok(product);
        }
    }
}
