using System;
using System.Collections.Generic;
using System.Text;
using Microsoft.Extensions.Configuration;
using Mirai.Application.Interfaces.Services;
using System.Text.Json;

namespace Mirai.Infastructure.Services
{

    public class TurnstileService : ITurnstileService
    {
        private readonly HttpClient _httpClient;
        private readonly IConfiguration _configuration;

        public TurnstileService(
            HttpClient httpClient,
            IConfiguration configuration)
        {
            _httpClient = httpClient;
            _configuration = configuration;
        }

        public async Task<bool> VerifyAsync(string token)
        {
            var secretKey = _configuration["CloudflareTurnstile:SecretKey"];

            var form = new Dictionary<string, string>
        {
            { "secret", secretKey! },
            { "response", token }
        };

            var response = await _httpClient.PostAsync(
                "https://challenges.cloudflare.com/turnstile/v0/siteverify",
                new FormUrlEncodedContent(form)
            );

            var json = await response.Content.ReadAsStringAsync();

            return json.Contains("\"success\":true");
        }
    }
}
