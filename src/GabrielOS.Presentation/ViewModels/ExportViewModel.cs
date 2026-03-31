using System.IO;
using System.Windows;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using GabrielOS.Application.Services;
using GabrielOS.Domain.Interfaces;
using Microsoft.Win32;

namespace GabrielOS.Presentation.ViewModels;

public partial class ExportViewModel : ObservableObject
{
    private readonly ExportService _exportService;
    private readonly IUserRepository _userRepo;
    private Guid _userId;

    [ObservableProperty] private string _lastExportMessage = string.Empty;
    [ObservableProperty] private bool _isExporting;

    public ExportViewModel(ExportService exportService, IUserRepository userRepo)
    {
        _exportService = exportService;
        _userRepo = userRepo;
        _ = InitAsync();
    }

    private async Task InitAsync()
    {
        var user = await _userRepo.GetDefaultUserAsync();
        if (user != null) _userId = user.Id;
    }

    [RelayCommand]
    private async Task BackupDatabaseAsync()
    {
        var dialog = new SaveFileDialog
        {
            Title = "Backup Database",
            Filter = "SQLite Database (*.db)|*.db",
            FileName = $"gabrielos_backup_{DateTime.Now:yyyyMMdd_HHmm}.db",
        };
        if (dialog.ShowDialog() != true) return;

        IsExporting = true;
        try
        {
            await _exportService.BackupDatabaseAsync(dialog.FileName);
            LastExportMessage = $"Database backed up to: {dialog.FileName}";
        }
        catch (Exception ex) { LastExportMessage = $"Backup failed: {ex.Message}"; }
        finally { IsExporting = false; }
    }

    [RelayCommand]
    private async Task ExportJsonAsync()
    {
        var dialog = new SaveFileDialog
        {
            Title = "Export Data as JSON",
            Filter = "JSON File (*.json)|*.json",
            FileName = $"gabrielos_export_{DateTime.Now:yyyyMMdd}.json",
        };
        if (dialog.ShowDialog() != true) return;

        IsExporting = true;
        try
        {
            await _exportService.ExportJsonAsync(_userId, dialog.FileName);
            LastExportMessage = $"Data exported to: {dialog.FileName}";
        }
        catch (Exception ex) { LastExportMessage = $"Export failed: {ex.Message}"; }
        finally { IsExporting = false; }
    }

    [RelayCommand]
    private async Task ExportJournalMarkdownAsync()
    {
        var dialog = new SaveFileDialog
        {
            Title = "Export Journal as Markdown",
            Filter = "Markdown File (*.md)|*.md",
            FileName = $"journal_export_{DateTime.Now:yyyyMMdd}.md",
        };
        if (dialog.ShowDialog() != true) return;

        IsExporting = true;
        try
        {
            await _exportService.ExportJournalMarkdownAsync(_userId, dialog.FileName);
            LastExportMessage = $"Journal exported to: {dialog.FileName}";
        }
        catch (Exception ex) { LastExportMessage = $"Export failed: {ex.Message}"; }
        finally { IsExporting = false; }
    }
}
