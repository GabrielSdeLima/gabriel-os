using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using GabrielOS.Application.Services;
using GabrielOS.Presentation.Services;

namespace GabrielOS.Presentation.ViewModels;

public partial class SettingsViewModel : ObservableObject
{
    private readonly SettingsService _settingsService;
    private readonly ThemeService _themeService;

    [ObservableProperty] private string _apiKey = string.Empty;
    [ObservableProperty] private string _aiModel = string.Empty;
    [ObservableProperty] private bool _isSaved;
    [ObservableProperty] private bool _isDarkTheme;

    public IReadOnlyList<string> AvailableModels { get; } = new[]
    {
        "claude-haiku-4-5-20251001",
        "claude-sonnet-4-6",
        "claude-opus-4-6",
    };

    public SettingsViewModel(SettingsService settingsService, ThemeService themeService)
    {
        _settingsService = settingsService;
        _themeService = themeService;
        Load();
    }

    private void Load()
    {
        ApiKey = _settingsService.GetApiKey() ?? string.Empty;
        AiModel = _settingsService.GetAIModel();
        IsDarkTheme = _themeService.CurrentTheme == ThemeMode.Dark;
        IsSaved = false;
    }

    [RelayCommand]
    private void ToggleTheme()
    {
        _themeService.Toggle();
        IsDarkTheme = _themeService.CurrentTheme == ThemeMode.Dark;
    }

    [RelayCommand]
    private void Save()
    {
        _settingsService.SetApiKey(string.IsNullOrWhiteSpace(ApiKey) ? null : ApiKey.Trim());
        _settingsService.SetAIModel(AiModel);
        IsSaved = true;
    }

    [RelayCommand]
    private void Clear()
    {
        ApiKey = string.Empty;
        _settingsService.SetApiKey(null);
        IsSaved = false;
    }
}
