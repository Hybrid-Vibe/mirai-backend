using Mirai.Application.DTO;
using Mirai.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Text;

namespace Mirai.Application.Interfaces.Repositories
{
    public interface IReviewRepository
    {
        Task<Review> CreateReviewAsync(CreateReviewRequest createReviewRequest);
        Task<Review?> GetReviewByUserId(string userId);
    }
}
