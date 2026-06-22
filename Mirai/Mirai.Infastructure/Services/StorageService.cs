using System;
using System.Collections.Generic;
using System.Text;
using Microsoft.AspNetCore.Http;
using Mirai.Application.Interfaces.Services;
using Supabase;


namespace Mirai.Infastructure.Services
{
    public class StorageService : IStorageService
    {
        private readonly Client _client;

        public StorageService(SupabaseClientService supabase)
        {
            _client = supabase.Client;
        }

        public async Task<string> UploadImage(IFormFile file)
        {
            var bucket = _client.Storage.From("images");

            using var ms = new MemoryStream();
            await file.CopyToAsync(ms);

            var fileName = $"uploads/{Guid.NewGuid()}.jpg";

            await bucket.Upload(ms.ToArray(), fileName);

            return bucket.GetPublicUrl(fileName);
        }

        public async Task<string> UploadImageByAI(byte[] fileBytes, string userId, string extension, CancellationToken cancellationToken = default)
        {
            var bucket = _client.Storage.From("images");

            var safeExt = extension.Trim('.');

            if (safeExt != "png" && safeExt != "jpg" && safeExt != "jpeg" && safeExt != "webp")
                safeExt = "png";

            var fileName = $"ai-generated/{userId}/{Guid.NewGuid():N}.{safeExt}";

            await bucket.Upload(fileBytes, fileName);

            return bucket.GetPublicUrl(fileName);
        }
    }
}
