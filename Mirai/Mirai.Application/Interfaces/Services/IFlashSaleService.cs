using Mirai.Application.DTO;
using System;
using System.Collections.Generic;
using System.Text;

namespace Mirai.Application.Interfaces.Services
{
    public interface IFlashSaleService
    {
        Task<CreateFlashSaleRequestDto> CreateFlashSaleDtosAsync(CreateFlashSaleRequestDto createFlashSaleRequestDto);
        Task UpdateFlashSaleAsync(string flashSaleId, UpdateFlashSaleRequest request);
        Task DeleteFlashSaleAsync(string flashSaleId);
    }
}
