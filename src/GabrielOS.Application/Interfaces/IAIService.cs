namespace GabrielOS.Application.Interfaces;

public interface IAIService
{
    bool IsConfigured { get; }
    Task<string?> CompleteAsync(string systemPrompt, string userPrompt, int maxTokens = 1024);
}
