using Mirai.Application.DTO;
using System;
using System.Collections.Generic;
using System.Text;

namespace Mirai.Application.Interfaces.Services
{
    public interface IReviewService
    {
        Task<bool> CreateReview(string userId, CreateReviewRequest createReviewRequest);
    }
}
