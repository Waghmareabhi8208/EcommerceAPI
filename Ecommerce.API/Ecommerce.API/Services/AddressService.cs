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
        public async Task<AddressResponseDto?> GetAddressByIdAsync(int userId, int addressId)
        {
           var address = await _context.Addresses
                .FirstOrDefaultAsync(x => x.Id == addressId && x.UserId == userId);

            if(address == null)
                return null;

            return new AddressResponseDto
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
            };
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

        public Task<bool> SetDefaultAddressAsync(int userId, int addressId)
        {
            throw new NotImplementedException();
        }

        public async Task<bool> UpdateAddressAsync(int userId, int addressId, UpdateAddressDto dto)
        {
            var address = await _context.Addresses
                .FirstOrDefaultAsync(x => x.Id == addressId && x.UserId == userId);

            if(address == null)
                return false;

            // Handle default logic
            if (dto.IsDefault)
            {
                var existingDefaults =
                    await _context.Addresses
                        .Where(x =>
                            x.UserId == userId &&
                            x.IsDefault)
                        .ToListAsync();

                foreach (var item in existingDefaults)
                {
                    item.IsDefault = false;
                }
            }

            address.FullName = dto.FullName;

            address.PhoneNumber = dto.PhoneNumber;

            address.Street = dto.Street;

            address.City = dto.City;

            address.State = dto.State;

            address.PostalCode = dto.PostalCode;

            address.Country = dto.Country;

            address.IsDefault = dto.IsDefault;

            await _context.SaveChangesAsync();

            return true;
        }
        public Task<bool> DeleteAddressAsync(int userId, int addressId)
        {
            throw new NotImplementedException();
        }
    }
}
