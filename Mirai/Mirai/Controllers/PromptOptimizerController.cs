using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Mirai.Application.Interfaces.Services;

namespace Mirai.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class PromptOptimizerController : ControllerBase
    {
        private readonly IPromptOptimizerService _promptOptimizerService;
        private readonly ILanguageDetector _languageDetector;
        public PromptOptimizerController(IPromptOptimizerService promptOptimizerService, ILanguageDetector languageDetector)
        {
            _promptOptimizerService = promptOptimizerService;
            _languageDetector = languageDetector;
        }

        [HttpPost("optimize")]
        public async Task<IActionResult> Optimize([FromBody] string prompt, CancellationToken cancellationToken)
        {
            var optimizedPrompt = await _promptOptimizerService.OptimizeAsync(prompt, cancellationToken);
            Console.WriteLine(
    _languageDetector.IsVietnamese(prompt));
            return Ok(optimizedPrompt);
        }
    }
}
