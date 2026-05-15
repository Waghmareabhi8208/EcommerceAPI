using Ecommerce.API.Data;
using Ecommerce.API.DTOs.Address;
using Ecommerce.API.Entities;
using Ecommerce.API.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace Ecommerce.API.Services
{
    public class AddressService : IAddressService
    {
        private readonly AppDbContext _context;
        public AddressService(AppDbContext context)
        {
            _context = context;
        }
        public async Task AddAddressAsync(int userId, CreateAddressDto dto)
        {
           if(dto.IsDefault)
           {
                var existingDefaults =
                    await _context.Addresses
                    .Where(x => x.UserId == userId && x.IsDefault)
                    .ToListAsync();

                foreach (var existingAddress in existingDefaults)
                {
                    existingAddress.IsDefault = false;
                }
           }

            var address = new Address
            {
                UserId = userId,

                FullName = dto.FullName,

                PhoneNumber = dto.PhoneNumber,

                Street = dto.Street,

                City = dto.City,

                State = dto.State,

                PostalCode = dto.PostalCode,

                Country = dto.Country,

                IsDefault = dto.IsDefault
            };
            _context.Addresses.Add(address);
            await _context.SaveChangesAsync(); 
        }

        public async Task<List<AddressResponseDto>> GetAddressesAsync(int userId)
        {
            var addresses = await _context.Addresses
                .Where(x =>x.UserId == userId)
                .ToListAsync();

            return addresses.Select(address =>
                new AddressResponseDto
                {
                    Id = address.Id,

                    FullName = address.FullName,

                    PhoneNumber = address.PhoneNumber,

                    Street = address.Street,

                    City = address.City,

                    State = address.State,

                    PostalCode = address.PostalCode,

                    Country = address.Country,

                    IsDefault = address.IsDefault

                }).ToList();
        }
    }
}
