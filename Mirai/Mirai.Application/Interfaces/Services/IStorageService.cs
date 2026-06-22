using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.Text;

namespace Mirai.Application.Interfaces.Services
{
    public interface IStorageService
    {
        Task<string> UploadImage(IFormFile file);
        Task<string> UploadImageByAI(byte[] fileBytes, string userId, string extension, CancellationToken cancellationToken = default);
    }
}
