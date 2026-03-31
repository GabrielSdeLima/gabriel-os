namespace GabrielOS.Application.DTOs;

public record SearchResult(
    string EntityType,
    Guid Id,
    string Title,
    string Snippet,
    DateTime Date);
