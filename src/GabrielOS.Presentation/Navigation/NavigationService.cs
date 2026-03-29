using CommunityToolkit.Mvvm.ComponentModel;
using Microsoft.Extensions.DependencyInjection;

namespace GabrielOS.Presentation.Navigation;

public interface INavigationService
{
    ObservableObject CurrentViewModel { get; }
    void NavigateTo<TViewModel>() where TViewModel : ObservableObject;
    event Action? CurrentViewModelChanged;
}

public class NavigationService : INavigationService
{
    private readonly IServiceProvider _serviceProvider;
    private ObservableObject _currentViewModel = null!;

    public NavigationService(IServiceProvider serviceProvider)
    {
        _serviceProvider = serviceProvider;
    }

    public ObservableObject CurrentViewModel
    {
        get => _currentViewModel;
        private set
        {
            _currentViewModel = value;
            CurrentViewModelChanged?.Invoke();
        }
    }

    public event Action? CurrentViewModelChanged;

    public void NavigateTo<TViewModel>() where TViewModel : ObservableObject
    {
        var viewModel = (ObservableObject)_serviceProvider.GetRequiredService(typeof(TViewModel));
        CurrentViewModel = viewModel;
    }
}

