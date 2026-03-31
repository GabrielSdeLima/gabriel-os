using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Text.Json;
using GabrielOS.Application.Interfaces;
using GabrielOS.Application.Services;

namespace GabrielOS.Infrastructure.AI;

public class AnthropicService : IAIService
{
    private static readonly HttpClient _httpClient = new()
    {
        BaseAddress = new Uri("https://api.anthropic.com"),
        Timeout = TimeSpan.FromSeconds(60),
    };

    private readonly SettingsService _settings;

    public bool IsConfigured => !string.IsNullOrWhiteSpace(_settings.GetApiKey());

    public AnthropicService(SettingsService settings)
    {
        _settings = settings;
    }

    public async Task<string?> CompleteAsync(string systemPrompt, string userPrompt, int maxTokens = 1024)
    {
        var apiKey = _settings.GetApiKey();
        if (string.IsNullOrWhiteSpace(apiKey)) return null;

        var model = _settings.GetAIModel();

        var requestBody = new
        {
            model,
            max_tokens = maxTokens,
            system = systemPrompt,
            messages = new[] { new { role = "user", content = userPrompt } }
        };

        var request = new HttpRequestMessage(HttpMethod.Post, "/v1/messages");
        request.Headers.Add("x-api-key", apiKey);
        request.Headers.Add("anthropic-version", "2023-06-01");
        request.Content = new StringContent(
            JsonSerializer.Serialize(requestBody),
            Encoding.UTF8,
            "application/json");

        var response = await _httpClient.SendAsync(request);
        var body = await response.Content.ReadAsStringAsync();

        if (!response.IsSuccessStatusCode)
            throw new InvalidOperationException($"Anthropic API error {response.StatusCode}: {body}");

        using var doc = JsonDocument.Parse(body);
        return doc.RootElement
            .GetProperty("content")[0]
            .GetProperty("text")
            .GetString();
    }
}
