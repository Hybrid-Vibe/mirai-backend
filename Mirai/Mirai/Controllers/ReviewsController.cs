using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Mirai.Application.DTO;
using Mirai.Application.Interfaces.Services;

namespace Mirai.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ReviewsController : ControllerBase
    {
        private readonly IReviewService _reviewService;
        private readonly IUserService _userService; 
       
        public ReviewsController(IReviewService reviewService, IUserService userService)
        {
            _reviewService = reviewService;
            _userService = userService;
        }

        [HttpPost("Create-review")]
        public async Task<IActionResult> CreateReview([FromBody] CreateReviewRequest createReviewRequest)
        {
            var user = await _userService.GetUserByIdAsync(createReviewRequest.UserId);
            if(user == null) {
                return BadRequest("User not found");
            }
            var result = await _reviewService.CreateReview(user.UserId, createReviewRequest);
            if (!result)
            {
                return BadRequest("Failed to create review");
            }
            return Ok("Review created successfully");
        }
    }
}
