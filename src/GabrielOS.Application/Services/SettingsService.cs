using System.IO;
using System.Text.Json;

namespace GabrielOS.Application.Services;

public class SettingsService
{
    private readonly string _settingsPath;
    private AppSettings _settings;

    public SettingsService()
    {
        var appDataPath = Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData);
        var appFolder = Path.Combine(appDataPath, "GabrielOS");
        Directory.CreateDirectory(appFolder);
        _settingsPath = Path.Combine(appFolder, "settings.json");
        _settings = Load();
    }

    public string? GetApiKey() => _settings.AnthropicApiKey;
    public string GetAIModel() => _settings.AIModel ?? "claude-haiku-4-5-20251001";

    public void SetApiKey(string? key)
    {
        _settings.AnthropicApiKey = key?.Trim();
        Save();
    }

    public void SetAIModel(string model)
    {
        _settings.AIModel = model;
        Save();
    }

    public string? GetTheme() => _settings.Theme;

    public void SetTheme(string theme)
    {
        _settings.Theme = theme;
        Save();
    }

    private AppSettings Load()
    {
        try
        {
            if (!File.Exists(_settingsPath)) return new AppSettings();
            var json = File.ReadAllText(_settingsPath);
            return JsonSerializer.Deserialize<AppSettings>(json) ?? new AppSettings();
        }
        catch { return new AppSettings(); }
    }

    private void Save()
    {
        var json = JsonSerializer.Serialize(_settings, new JsonSerializerOptions { WriteIndented = true });
        File.WriteAllText(_settingsPath, json);
    }

    private class AppSettings
    {
        public string? AnthropicApiKey { get; set; }
        public string? AIModel { get; set; }
        public string? Theme { get; set; }
    }
}
