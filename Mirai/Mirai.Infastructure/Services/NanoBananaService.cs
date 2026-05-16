using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Mirai.Application.DTO;
using Mirai.Application.Interfaces.Services;
using System;
using System.Net.Http;
using System.Text;
using System.Text.Json;
using System.Threading;
using System.Threading.Tasks;

namespace Mirai.Infastructure.Services;

public class NanoBananaService : INanoBananaService
{
    private readonly HttpClient _httpClient;
    private readonly IConfiguration _configuration;
    private readonly ILogger<NanoBananaService> _logger;

    public NanoBananaService(HttpClient httpClient, IConfiguration configuration, ILogger<NanoBananaService> logger)
    {
        _httpClient = httpClient;
        _configuration = configuration;
        _logger = logger;
    }

    public async Task<NanoBananaResponseDto> GenerateImageAsync(NanoBananaRequestDto request, CancellationToken cancellationToken = default)
    {
        try
        {
            var jsonContent = JsonSerializer.Serialize(request);
            var content = new StringContent(jsonContent, Encoding.UTF8, "application/json");

            var apiKey = _configuration["NanoBanana:ApiKey"];
            const string modelPath = "/v1beta/models/gemini-1.5-flash-latest:generateContent";
            var requestUrl = $"{modelPath}?key={apiKey}";

            _logger.LogInformation("Calling Google Gemini API at: {Path}", modelPath);
            var response = await _httpClient.PostAsync(requestUrl, content, cancellationToken);

            if (response.IsSuccessStatusCode)
            {
                var responseContent = await response.Content.ReadAsStringAsync(cancellationToken);
                var result = JsonSerializer.Deserialize<NanoBananaResponseDto>(responseContent, new JsonSerializerOptions
                {
                    PropertyNameCaseInsensitive = true
                });
                
                _logger.LogInformation("NanoBanana API call successful");
                return result ?? new NanoBananaResponseDto { Success = false, ErrorMessage = "Failed to parse response" };
            }
            else
            {
                var errorContent = await response.Content.ReadAsStringAsync(cancellationToken);
                _logger.LogError("NanoBanana API error: {StatusCode} - {Content}", response.StatusCode, errorContent);
                
                return new NanoBananaResponseDto 
                { 
                    Success = false, 
                    ErrorMessage = $"API Error: {response.StatusCode} - {errorContent}" 
                };
            }
        }
        catch (OperationCanceledException)
        {
            _logger.LogWarning("NanoBanana API call was cancelled");
            throw;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error calling NanoBanana API");
            return new NanoBananaResponseDto 
            { 
                Success = false, 
                ErrorMessage = $"Exception: {ex.Message}" 
            };
        }
    }

    public async Task<NanoBananaResponseDto> GetImageStatusAsync(string requestId, CancellationToken cancellationToken = default)
    {
        try
        {
            _logger.LogInformation("Checking NanoBanana image status for RequestId: {RequestId}", requestId);
            var response = await _httpClient.GetAsync($"/v1/image/status/{requestId}", cancellationToken);
            
            if (response.IsSuccessStatusCode)
            {
                var responseContent = await response.Content.ReadAsStringAsync(cancellationToken);
                var result = JsonSerializer.Deserialize<NanoBananaResponseDto>(responseContent, new JsonSerializerOptions
                {
                    PropertyNameCaseInsensitive = true
                });
                
                return result ?? new NanoBananaResponseDto { Success = false, ErrorMessage = "Failed to parse response" };
            }
            else
            {
                var errorContent = await response.Content.ReadAsStringAsync(cancellationToken);
                _logger.LogError("NanoBanana status check error: {StatusCode} - {Content}", response.StatusCode, errorContent);
                
                return new NanoBananaResponseDto 
                { 
                    Success = false, 
                    ErrorMessage = $"Status Check Error: {response.StatusCode}" 
                };
            }
        }
        catch (OperationCanceledException)
        {
            _logger.LogWarning("NanoBanana status check was cancelled");
            throw;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error checking NanoBanana image status");
            return new NanoBananaResponseDto 
            { 
                Success = false, 
                ErrorMessage = $"Exception: {ex.Message}" 
            };
        }
    }
}
