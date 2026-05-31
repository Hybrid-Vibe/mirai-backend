using Mirai.Application.DTO;
using System;
using System.Collections.Generic;
using System.Text;

namespace Mirai.Application.Interfaces.Repositories
{
    public interface IFlashSaleRepository
    {
        Task<CreateFlashSaleRequestDto> CreateFlashSaleDtosAsync(CreateFlashSaleRequestDto createFlashSaleRequestDto);
        Task UpdateFlashSaleAsync(string flashSaleId, UpdateFlashSaleRequest request);
        Task DeleteFlashSaleAsync(string flashSaleId);
    }
}
