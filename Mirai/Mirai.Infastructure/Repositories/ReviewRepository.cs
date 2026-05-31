using Microsoft.EntityFrameworkCore;
using Mirai.Application.DTO;
using Mirai.Application.Interfaces.Repositories;
using Mirai.Domain.Entities;
using Mirai.Infastructure.Data;
using System;
using System.Collections.Generic;
using System.Text;

namespace Mirai.Infastructure.Repositories
{
    public class ReviewRepository : GenericRepository<Review>, IReviewRepository
    {
        public ReviewRepository(AppDbContext context) : base(context)
        {
            
        }
        public async Task<Review> CreateReviewAsync(CreateReviewRequest createReviewRequest)
        {
            var review = new Review()
            {
                ReviewId = Guid.NewGuid().ToString(),
                UserId = createReviewRequest.UserId, 
                ProductId = createReviewRequest.ProductId,
                VariantId = createReviewRequest.VariantId,
                Rating = createReviewRequest.Rating,
                Title = createReviewRequest.Title,
                Comment = createReviewRequest.Comment,
                IsApproved = true, 
                CreatedAt = DateTime.Now
            };
            await _context.Reviews.AddAsync(review);
            await _context.SaveChangesAsync();
            return review;
        }

        public async Task<Review?> GetReviewByUserId(string userId)
        {
            return await _context.Reviews.FirstOrDefaultAsync(r => r.UserId == userId);
        }
    }
}
