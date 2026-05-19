using Ecommerce.API.DTOs.Address;

namespace Ecommerce.API.Interfaces
{
    public interface IAddressService
    {
        Task AddAddressAsync(int userId, CreateAddressDto dto);
        Task<List<AddressResponseDto>> GetAddressesAsync(int userId);
        Task<AddressResponseDto?> GetAddressByIdAsync(int userId,int addressId);
        Task<bool> UpdateAddressAsync(int userId,int addressId,UpdateAddressDto dto);
        Task<bool> DeleteAddressAsync(int userId,int addressId);
        Task<bool> SetDefaultAddressAsync(int userId,int addressId);
    }
}
