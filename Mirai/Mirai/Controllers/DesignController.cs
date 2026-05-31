using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Mirai.Filters;

namespace Mirai.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class DesignController : ControllerBase
    {
        [HttpPost("Captcha")]


        [ValidateTurnstile]

        public IActionResult Generate()
        {
            return Ok(new
            {
                message = "Success"
            });
        }
    }
}
