using Mirai.Application.DTO;
using Mirai.Application.Interfaces.Repositories;
using Mirai.Application.Interfaces.Services;
using System;
using System.Collections.Generic;
using System.Text;

namespace Mirai.Infastructure.Services
{
    public class FlashSaleService : IFlashSaleService
    {
        private readonly IUnitOfWork _unitOfWork;
        public FlashSaleService(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        public async Task<CreateFlashSaleRequestDto> CreateFlashSaleDtosAsync(CreateFlashSaleRequestDto createFlashSaleRequestDto)
        {
            return await _unitOfWork.FlashSaleRepository.CreateFlashSaleDtosAsync(createFlashSaleRequestDto);
        }

        public async Task DeleteFlashSaleAsync(string flashSaleId)
        {
            await _unitOfWork.FlashSaleRepository.DeleteFlashSaleAsync(flashSaleId);
        }

        public async Task UpdateFlashSaleAsync(string flashSaleId, UpdateFlashSaleRequest request)
        {
            await _unitOfWork.FlashSaleRepository.UpdateFlashSaleAsync(flashSaleId, request);
        }
        
    }
}
