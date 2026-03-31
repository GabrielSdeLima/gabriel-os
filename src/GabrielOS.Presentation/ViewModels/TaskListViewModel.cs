using System.Collections.ObjectModel;
using System.Windows;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using GabrielOS.Application.Services;
using GabrielOS.Domain.Entities;
using GabrielOS.Domain.Enums;
using GabrielOS.Domain.Interfaces;

namespace GabrielOS.Presentation.ViewModels;

public partial class TaskListViewModel : ObservableObject
{
    private readonly TaskItemService _taskService;
    private readonly GoalService _goalService;
    private readonly IUserRepository _userRepo;
    private Guid _userId;
    private Guid? _editingId;

    [ObservableProperty] private ObservableCollection<TaskItem> _tasks = new();
    [ObservableProperty] private ObservableCollection<Goal> _goals = new();

    // Form
    [ObservableProperty] private string _newTitle = string.Empty;
    [ObservableProperty] private TaskItemStatus _newStatus = TaskItemStatus.Next;
    [ObservableProperty] private GoalPriority? _newPriority;
    [ObservableProperty] private Goal? _newGoal;
    [ObservableProperty] private bool _newIsNextAction;

    // State
    [ObservableProperty] private bool _isEditMode;
    [ObservableProperty] private bool _isLoading = true;

    public IReadOnlyList<TaskItemStatus> StatusValues { get; } = Enum.GetValues<TaskItemStatus>();
    public IReadOnlyList<GoalPriority?> PriorityValues { get; } = new GoalPriority?[] { null, GoalPriority.P1, GoalPriority.P2, GoalPriority.P3, GoalPriority.P4 };
    public string FormTitle => IsEditMode ? "Edit Task" : "New Task";
    public string SaveButtonText => IsEditMode ? "Save Changes" : "Add Task";

    partial void OnIsEditModeChanged(bool value)
    {
        OnPropertyChanged(nameof(FormTitle));
        OnPropertyChanged(nameof(SaveButtonText));
    }

    public TaskListViewModel(TaskItemService taskService, GoalService goalService, IUserRepository userRepo)
    {
        _taskService = taskService;
        _goalService = goalService;
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

            var goals = await _goalService.GetByUserAsync(_userId);
            Goals = new ObservableCollection<Goal>(goals.Where(g => g.Status == GoalStatus.Active));

            var tasks = await _taskService.GetByUserAsync(_userId);
            Tasks = new ObservableCollection<TaskItem>(tasks);
        }
        finally { IsLoading = false; }
    }

    [RelayCommand]
    private async Task SaveTaskAsync()
    {
        if (string.IsNullOrWhiteSpace(NewTitle)) return;

        if (IsEditMode && _editingId.HasValue)
        {
            var task = Tasks.FirstOrDefault(t => t.Id == _editingId.Value);
            if (task == null) return;

            task.Title = NewTitle.Trim();
            task.Status = NewStatus;
            task.Priority = NewPriority;
            task.GoalId = NewGoal?.Id;
            task.IsNextAction = NewIsNextAction;

            await _taskService.UpdateAsync(task);
            CancelEdit();
        }
        else
        {
            var task = new TaskItem
            {
                UserId = _userId,
                Title = NewTitle.Trim(),
                Status = NewStatus,
                Priority = NewPriority,
                GoalId = NewGoal?.Id,
                IsNextAction = NewIsNextAction,
            };
            await _taskService.CreateAsync(task);
            ResetForm();
        }

        await LoadAsync();
    }

    [RelayCommand]
    private async Task ToggleDoneAsync(TaskItem? task)
    {
        if (task == null) return;
        task.Status = task.Status == TaskItemStatus.Done ? TaskItemStatus.Next : TaskItemStatus.Done;
        await _taskService.UpdateAsync(task);
        await LoadAsync();
    }

    [RelayCommand]
    private void BeginEditTask(TaskItem? task)
    {
        if (task == null) return;
        _editingId = task.Id;
        NewTitle = task.Title;
        NewStatus = task.Status;
        NewPriority = task.Priority;
        NewGoal = Goals.FirstOrDefault(g => g.Id == task.GoalId);
        NewIsNextAction = task.IsNextAction;
        IsEditMode = true;
    }

    [RelayCommand]
    private void CancelEdit()
    {
        IsEditMode = false;
        _editingId = null;
        ResetForm();
    }

    [RelayCommand]
    private async Task DeleteTaskAsync(TaskItem? task)
    {
        if (task == null) return;
        var result = MessageBox.Show($"Delete task \"{task.Title}\"?",
            "Confirm Delete", MessageBoxButton.YesNo, MessageBoxImage.Warning);
        if (result != MessageBoxResult.Yes) return;

        await _taskService.DeleteAsync(task.Id);
        await LoadAsync();
    }

    private void ResetForm()
    {
        NewTitle = string.Empty;
        NewStatus = TaskItemStatus.Next;
        NewPriority = null;
        NewGoal = null;
        NewIsNextAction = false;
    }
}
