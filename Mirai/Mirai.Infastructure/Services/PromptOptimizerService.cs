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

        private const string PrintableFallbackSuffix =
            ", flat 2D printable artwork texture, vertical 9:16 composition, " +
            "no phone, no phone case, no mockup, no device frame, " +
            "top 30% clean low-detail, modern clean print-ready art";
        public PromptOptimizerService(
            HttpClient httpClient,
            IOptions<GroqOptions> options)
        {
            _httpClient = httpClient;

            _httpClient.Timeout =
                TimeSpan.FromSeconds(15);

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
                You are an AI prompt engineer for printable phone case artwork.

                Convert Vietnamese user descriptions into concise English prompts for AI image generation.

                Rules:
                - Preserve the user's original subject and intent.
                - Output only a flat 2D printable artwork texture.
                - Do not generate a phone, phone case shell, camera hole, camera lens, product mockup, hand, device frame, or product render.
                - Use vertical 9:16 composition.
                - Place the main subject in the bottom center or lower-middle safe area.
                - Keep the top 30% clean and low-detail for the real phone camera cluster that will be overlaid later.
                - Keep the upper-left camera area free of faces, text, logos, and important details.
                - Add modern, clean, print-ready art direction.
                - Do not add text unless the user explicitly requests text.
                - Return only the final English prompt.
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


            try
            {
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
                        $"Groq error {response.StatusCode}: {error}");
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

                return string.IsNullOrWhiteSpace(result)
                    ? prompt + PrintableFallbackSuffix
                    : result.Trim();

            }
            catch (Exception ex)
            {
                return prompt + PrintableFallbackSuffix;
            }
        }
    }
}
