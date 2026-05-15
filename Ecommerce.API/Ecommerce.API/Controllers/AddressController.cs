using Ecommerce.API.DTOs.Address;
using Ecommerce.API.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace Ecommerce.API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    [Authorize]
    public class AddressController : ControllerBase
    {
        private readonly IAddressService _addressService;

        public AddressController(
            IAddressService addressService)
        {
            _addressService = addressService;
        }

        // Endpoint to add Address
        [HttpPost]
        public async Task<IActionResult> AddAddress(
            CreateAddressDto dto)
        {
            int userId = int.Parse(
                User.FindFirst(
                    ClaimTypes.NameIdentifier)!
                    .Value);

            await _addressService.AddAddressAsync(
                userId,
                dto);

            return Ok("Address added");
        }

        // Endpoint to get addresses
        [HttpGet]
        public async Task<IActionResult> GetAddresses()
        {
            int userId = int.Parse(
                User.FindFirst(
                    ClaimTypes.NameIdentifier)!
                    .Value);

            var addresses =
                await _addressService
                    .GetAddressesAsync(userId);

            return Ok(addresses);
        }
    }
}