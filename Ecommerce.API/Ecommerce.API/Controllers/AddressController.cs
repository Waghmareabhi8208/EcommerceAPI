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

        // Endpoint to get single address
        [HttpGet("{addressId}")]
        public async Task<IActionResult> GetAddressById(int addressId)
        {
            int userId = int.Parse(
                User.FindFirst(
                    ClaimTypes.NameIdentifier)!
                    .Value);

            var address =
                await _addressService.GetAddressByIdAsync(
                    userId,
                    addressId);

            if (address == null)
            {
                return NotFound();
            }

            return Ok(address);
        }

        // Api endpoint to update address
        [HttpPut("{addressId}")]
        public async Task<IActionResult> UpdateAddress(int addressId,UpdateAddressDto dto)
        {
            int userId = int.Parse(
                User.FindFirst(
                    ClaimTypes.NameIdentifier)!
                    .Value);

            bool updated =
                await _addressService.UpdateAddressAsync(
                    userId,
                    addressId,
                    dto);

            if (!updated)
            {
                return NotFound();
            }

            return Ok("Address updated");
        }
    }
}