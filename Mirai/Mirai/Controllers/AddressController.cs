using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Mirai.Application.DTO;
using Mirai.Application.Interfaces.Services;

namespace Mirai.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AddressController : ControllerBase
    {
        public readonly IAddressService _addressService;
        public AddressController(IAddressService addressService)
        {
            _addressService = addressService;
        }

        [HttpGet("Get-All-Addresses")]
        public async Task<IActionResult> GetAllAddressesAsync()
        {
            var addresses = await _addressService.GetAllAddressesAsync();
            return Ok(addresses);
        }

        [HttpGet("Get-Address-By-AddressId/{addressId}")]
        public async Task<IActionResult> GetAddressAsync(string addressId)
        {
            var addresses = await _addressService.GetAddressByIdAsync(addressId);
            return Ok(addresses);
        }
        [HttpGet("Get-Address-By-UserId/{userId}")]
        public async Task<IActionResult> GetAddressByUserIdAsync(string userId)
        {
            var addresses = await _addressService.GetAddressByUserIdAsync(userId);
            return Ok(addresses);
        }

        [HttpPost("Create-Address")]
        public async Task<IActionResult> CreateAddressAsync([FromBody] AddressDto addressDto)
        {
            var address = await _addressService.CreateAddressAsync(addressDto);
            if (address == null)
            {
                return BadRequest("Failed to create address.");
            }
            return Ok(address);
        }

        [HttpPut("Update-Address/{addressId}")]
        public async Task<IActionResult> UpdateAddressAsync([FromBody] UpdateAddressDto updateAddressDto, string addressId)
        {
            var address = await _addressService.UpdateAddressAsync(updateAddressDto, addressId);
            if (address == null)
            {
                return BadRequest("Failed to update address.");
            }
            return Ok(address);

        }
    }
}
