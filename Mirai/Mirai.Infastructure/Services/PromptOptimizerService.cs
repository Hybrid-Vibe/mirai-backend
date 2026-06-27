using Microsoft.Extensions.Options;
using Mirai.Application.DTO;
using Mirai.Application.Interfaces.Services;
using System;
using System.Collections.Generic;
using System.Text;
using System.Text.Json;

namespace Mirai.Infastructure.Services
{

    public class PromptOptimizerService : IPromptOptimizerService
    {
        private readonly HttpClient _httpClient;
        private readonly GroqOptions _options;


        public PromptOptimizerService(
            HttpClient httpClient,
            IOptions<GroqOptions> options)
        {
            _httpClient = httpClient;
            _options = options.Value;
        }


        public async Task<string> OptimizeAsync(
            string prompt,
            CancellationToken cancellationToken)
        {

            var request = new
            {
                model = _options.Model,

                messages = new[]
                {
                new
                {
                    role = "system",
                    content = """
                    You are an AI image prompt engineer.

                    Convert Vietnamese user descriptions into
                    professional English prompts for AI image generation.

                    Rules:
                    - Translate naturally
                    - Add visual details
                    - Improve lighting
                    - Improve composition
                    - Add camera/style details
                    - Keep original meaning
                    - Return ONLY the final prompt
                    """
                },

                new
                {
                    role = "user",
                    content = prompt
                }
            },

                temperature = 0.7,
                max_tokens = 500
            };


            using var httpRequest =
                new HttpRequestMessage(
                    HttpMethod.Post,
                    $"{_options.BaseUrl}chat/completions");


            httpRequest.Headers.Add(
                "Authorization",
                $"Bearer {_options.ApiKey}");


            httpRequest.Content =
                new StringContent(
                    JsonSerializer.Serialize(request),
                    Encoding.UTF8,
                    "application/json");


            var response =
                await _httpClient.SendAsync(
                    httpRequest,
                    cancellationToken);


            if (!response.IsSuccessStatusCode)
            {
                var error =
                    await response.Content.ReadAsStringAsync(
                        cancellationToken);

                throw new Exception(
                    $"OpenRouter error {response.StatusCode}: {error}");
            }

            var json =
                await response.Content.ReadAsStringAsync(
                    cancellationToken);


            using var doc =
                JsonDocument.Parse(json);


            var result =
                doc.RootElement
                .GetProperty("choices")[0]
                .GetProperty("message")
                .GetProperty("content")
                .GetString();


            return result?.Trim()
                ?? prompt;
        }
    }
}
