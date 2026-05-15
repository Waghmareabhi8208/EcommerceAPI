using Ecommerce.API.DTOs.Address;

namespace Ecommerce.API.Interfaces
{
    public interface IAddressService
    {
        Task AddAddressAsync(int userId, CreateAddressDto dto);
        Task<List<AddressResponseDto>> GetAddressesAsync(int userId);
    }
}
