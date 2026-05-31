using MediatR;
using Mirai.Application.DTO;
using Mirai.Application.Interfaces.Repositories;
using Mirai.Application.Interfaces.Services;
using System;
using System.Collections.Generic;
using System.Text;

namespace Mirai.Infastructure.Services
{
    public class ReviewService : IReviewService
    {
        private readonly IUnitOfWork _unitOfWork;

        public ReviewService(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        public async Task<bool> CreateReview(string userId, CreateReviewRequest createReviewRequest)
        {
            var user = await _unitOfWork.UserRepository.GetByIdAsync(userId);
            if (user == null)
            {
                return false;
            }
            var product = await _unitOfWork.ProductRepository.GetProductById(createReviewRequest.ProductId);
            if (product == null)
            {
                return false;
            }

            if (createReviewRequest.Rating < 1 || createReviewRequest.Rating > 5)
            {
                return false;
            }

            var review = await _unitOfWork.ReviewRepository.CreateReviewAsync(createReviewRequest);
            if (review == null)
            {
                return false;
            }

            var totalRating = product.RatingAvg.GetValueOrDefault() * product.RatingCount;
            var newRatingCount = product.RatingCount + 1;
            var newRatingAvg = (totalRating + review.Rating) / newRatingCount;

            await _unitOfWork.ProductRepository.UpdateProductStar(review.ProductId, newRatingAvg, newRatingCount);
            return true;
        }
    }
}
