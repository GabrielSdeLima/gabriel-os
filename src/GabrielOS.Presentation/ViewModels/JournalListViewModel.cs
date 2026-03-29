using System.Collections.ObjectModel;
using System.Windows;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using GabrielOS.Application.Services;
using GabrielOS.Domain.Entities;
using GabrielOS.Domain.Enums;
using GabrielOS.Domain.Interfaces;

namespace GabrielOS.Presentation.ViewModels;

public partial class JournalListViewModel : ObservableObject
{
    private readonly JournalService _journalService;
    private readonly IUserRepository _userRepo;
    private Guid _userId;
    private Guid? _editingEntryId;

    [ObservableProperty]
    private ObservableCollection<JournalEntry> _entries = new();

    // Form fields (shared for create and edit)
    [ObservableProperty] private string _newContent = string.Empty;
    [ObservableProperty] private string _newTitle = string.Empty;
    [ObservableProperty] private EntryType _newEntryType = EntryType.Reflection;
    [ObservableProperty] private bool _newIsSensitive;

    // State
    [ObservableProperty] private bool _isEditMode;
    [ObservableProperty] private string _searchQuery = string.Empty;
    [ObservableProperty] private bool _isLoading = true;

    public IReadOnlyList<EntryType> EntryTypes { get; } = Enum.GetValues<EntryType>();

    public string FormTitle => IsEditMode ? "Edit Entry" : "New Entry";
    public string SaveButtonText => IsEditMode ? "Save Changes" : "Save Entry";

    partial void OnIsEditModeChanged(bool value)
    {
        OnPropertyChanged(nameof(FormTitle));
        OnPropertyChanged(nameof(SaveButtonText));
    }

    public JournalListViewModel(JournalService journalService, IUserRepository userRepo)
    {
        _journalService = journalService;
        _userRepo = userRepo;
        _ = LoadAsync();
    }

    [RelayCommand]
    private async Task LoadAsync()
    {
        IsLoading = true;
        try
        {
            var user = await _userRepo.GetDefaultUserAsync();
            if (user == null) return;
            _userId = user.Id;

            var entries = await _journalService.GetEntriesAsync(_userId);
            Entries = new ObservableCollection<JournalEntry>(entries);
        }
        finally
        {
            IsLoading = false;
        }
    }

    [RelayCommand]
    private async Task SaveEntryAsync()
    {
        if (IsEditMode && _editingEntryId.HasValue)
        {
            var entry = Entries.FirstOrDefault(e => e.Id == _editingEntryId.Value);
            if (entry == null) return;

            entry.Title = string.IsNullOrWhiteSpace(NewTitle) ? null : NewTitle.Trim();
            entry.Content = NewContent;
            entry.EntryType = NewEntryType;
            entry.IsSensitive = NewIsSensitive;

            await _journalService.UpdateAsync(entry);
            CancelEdit();
        }
        else
        {
            if (string.IsNullOrWhiteSpace(NewContent)) return;

            var entry = new JournalEntry
            {
                UserId = _userId,
                Title = string.IsNullOrWhiteSpace(NewTitle) ? null : NewTitle.Trim(),
                Content = NewContent,
                EntryType = NewEntryType,
                IsSensitive = NewIsSensitive,
            };

            await _journalService.CreateAsync(entry);
            NewContent = string.Empty;
            NewTitle = string.Empty;
            NewEntryType = EntryType.Reflection;
            NewIsSensitive = false;
        }

        await LoadAsync();
    }

    [RelayCommand]
    private void BeginEditEntry(JournalEntry? entry)
    {
        if (entry == null) return;
        _editingEntryId = entry.Id;
        NewTitle = entry.Title ?? string.Empty;
        NewContent = entry.Content;
        NewEntryType = entry.EntryType;
        NewIsSensitive = entry.IsSensitive;
        IsEditMode = true;
    }

    [RelayCommand]
    private void CancelEdit()
    {
        IsEditMode = false;
        _editingEntryId = null;
        NewTitle = string.Empty;
        NewContent = string.Empty;
        NewEntryType = EntryType.Reflection;
        NewIsSensitive = false;
    }

    [RelayCommand]
    private async Task DeleteEntryAsync(JournalEntry? entry)
    {
        if (entry == null) return;

        var result = MessageBox.Show(
            "Delete this journal entry? This cannot be undone.",
            "Confirm Delete",
            MessageBoxButton.YesNo,
            MessageBoxImage.Warning);

        if (result != MessageBoxResult.Yes) return;

        await _journalService.DeleteAsync(entry.Id);
        await LoadAsync();
    }

    [RelayCommand]
    private async Task SearchAsync()
    {
        if (string.IsNullOrWhiteSpace(SearchQuery))
        {
            await LoadAsync();
            return;
        }

        var results = await _journalService.SearchAsync(_userId, SearchQuery);
        Entries = new ObservableCollection<JournalEntry>(results);
    }
}
