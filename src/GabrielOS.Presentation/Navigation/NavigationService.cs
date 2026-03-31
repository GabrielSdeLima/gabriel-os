using System.Windows;
using CommunityToolkit.Mvvm.ComponentModel;
using Microsoft.Extensions.DependencyInjection;

namespace GabrielOS.Presentation.Navigation;

public interface INavigationService
{
    ObservableObject CurrentViewModel { get; }
    bool CanGoBack { get; }
    void NavigateTo<TViewModel>() where TViewModel : ObservableObject;
    void GoBack();
    event Action? CurrentViewModelChanged;
}

public class NavigationService : INavigationService
{
    private readonly IServiceScopeFactory _scopeFactory;
    private readonly Stack<(IServiceScope Scope, ObservableObject Vm)> _backStack = new();
    private IServiceScope? _currentScope;
    private ObservableObject _currentViewModel = null!;
    private const int MaxStackDepth = 8;

    public NavigationService(IServiceScopeFactory scopeFactory)
    {
        _scopeFactory = scopeFactory;
    }

    public ObservableObject CurrentViewModel
    {
        get => _currentViewModel;
        private set { _currentViewModel = value; CurrentViewModelChanged?.Invoke(); }
    }

    public bool CanGoBack => _backStack.Count > 0;

    public event Action? CurrentViewModelChanged;

    public void NavigateTo<TViewModel>() where TViewModel : ObservableObject
    {
        if (!ConfirmNavigation()) return;

        // Push current to back stack
        if (_currentScope != null)
        {
            _backStack.Push((_currentScope, _currentViewModel));
            TrimBackStack();
        }

        var scope = _scopeFactory.CreateScope();
        _currentScope = scope;
        CurrentViewModel = (ObservableObject)scope.ServiceProvider.GetRequiredService(typeof(TViewModel));
    }

    public void GoBack()
    {
        if (!CanGoBack) return;
        if (!ConfirmNavigation()) return;

        _currentScope?.Dispose();
        var (scope, vm) = _backStack.Pop();
        _currentScope = scope;
        CurrentViewModel = vm;
    }

    private bool ConfirmNavigation()
    {
        if (_currentViewModel is IUnsavedChangesAware { HasUnsavedChanges: true })
        {
            var result = MessageBox.Show(
                "Há alterações não salvas. Deseja descartar e sair?",
                "Alterações não salvas",
                MessageBoxButton.YesNo,
                MessageBoxImage.Warning);
            return result == MessageBoxResult.Yes;
        }
        return true;
    }

    private void TrimBackStack()
    {
        while (_backStack.Count > MaxStackDepth)
        {
            // Remove and dispose the oldest (bottom) entry
            var items = _backStack.ToArray();
            _backStack.Clear();
            for (int i = 0; i < items.Length - 1; i++)
                _backStack.Push(items[i]);
            items[^1].Scope.Dispose();
        }
    }
}
