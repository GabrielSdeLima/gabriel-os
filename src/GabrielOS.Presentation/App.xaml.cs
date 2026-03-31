using System.IO;
using System.Windows;
using GabrielOS.Application.Interfaces;
using GabrielOS.Application.Services;
using GabrielOS.Domain.Interfaces;
using GabrielOS.Infrastructure.AI;
using GabrielOS.Infrastructure.Data;
using GabrielOS.Infrastructure.Repositories;
using GabrielOS.Infrastructure.Seeding;
using GabrielOS.Presentation.Navigation;
using GabrielOS.Presentation.ViewModels;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;

namespace GabrielOS.Presentation;

public partial class App : System.Windows.Application
{
    private ServiceProvider _serviceProvider = null!;

    protected override async void OnStartup(StartupEventArgs e)
    {
        base.OnStartup(e);

        var splash = new SplashWindow();
        splash.Show();

        var services = new ServiceCollection();
        ConfigureServices(services);
        _serviceProvider = services.BuildServiceProvider();

        await InitializeDatabaseAsync();

        // Keep splash visible long enough for the animation to be appreciated
        await Task.Delay(700);

        var mainWindow = _serviceProvider.GetRequiredService<MainWindow>();
        mainWindow.Opacity = 0;
        mainWindow.Show();
        mainWindow.PlayFadeIn();

        splash.PlayFadeOutAndClose();
    }

    private void ConfigureServices(IServiceCollection services)
    {
        // Database
        var dbPath = GetDatabasePath();
        services.AddDbContext<AppDbContext>(options =>
            options.UseSqlite($"Data Source={dbPath}"));

        // Repositories
        services.AddScoped<IUserRepository, UserRepository>();
        services.AddScoped<IPillarRepository, PillarRepository>();
        services.AddScoped<ICheckInRepository, CheckInRepository>();
        services.AddScoped<IJournalEntryRepository, JournalEntryRepository>();
        services.AddScoped<IGoalRepository, GoalRepository>();
        services.AddScoped<IDecisionRepository, DecisionRepository>();
        services.AddScoped<IWeeklyReviewRepository, WeeklyReviewRepository>();
        services.AddScoped<ICycleFocusRepository, CycleFocusRepository>();
        services.AddScoped<IPatternRepository, PatternRepository>();
        services.AddScoped<IMetricRepository, MetricRepository>();
        services.AddScoped<ITaskItemRepository, TaskItemRepository>();
        services.AddScoped<IMonthlyReviewRepository, MonthlyReviewRepository>();

        // Application Services
        services.AddScoped<PillarService>();
        services.AddScoped<CheckInService>();
        services.AddScoped<JournalService>();
        services.AddScoped<GoalService>();
        services.AddScoped<DecisionService>();
        services.AddScoped<ReviewService>();
        services.AddScoped<CycleFocusService>();
        services.AddScoped<AlertService>();
        services.AddScoped<SearchService>();
        services.AddScoped<TaskItemService>();
        services.AddScoped<PatternService>();
        services.AddScoped<MetricService>();
        services.AddScoped<MonthlyReviewService>();
        services.AddScoped<AIContextBuilder>();

        // Singleton: settings lives outside DI scope
        services.AddSingleton<SettingsService>();

        // Infrastructure Services
        services.AddScoped<ExportService>();
        services.AddSingleton<IAIService, AnthropicService>();

        // Navigation
        services.AddSingleton<INavigationService, NavigationService>();

        // ViewModels
        services.AddTransient<MainViewModel>();
        services.AddTransient<DashboardViewModel>();
        services.AddTransient<PillarListViewModel>();
        services.AddTransient<CheckInViewModel>();
        services.AddTransient<JournalListViewModel>();
        services.AddTransient<GoalListViewModel>();
        services.AddTransient<DecisionListViewModel>();
        services.AddTransient<WeeklyReviewViewModel>();
        services.AddTransient<CycleFocusViewModel>();
        services.AddTransient<TaskListViewModel>();
        services.AddTransient<PatternListViewModel>();
        services.AddTransient<MetricListViewModel>();
        services.AddTransient<SearchViewModel>();
        services.AddTransient<MonthlyReviewViewModel>();
        services.AddTransient<PillarTrendViewModel>();
        services.AddTransient<ExportViewModel>();
        services.AddTransient<SettingsViewModel>();

        // Windows
        services.AddTransient<MainWindow>();
    }

    private async Task InitializeDatabaseAsync()
    {
        using var scope = _serviceProvider.CreateScope();
        var context = scope.ServiceProvider.GetRequiredService<AppDbContext>();
        await context.Database.EnsureCreatedAsync();
        await DefaultDataSeeder.SeedAsync(context);
    }

    private static string GetDatabasePath()
    {
        var appDataPath = Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData);
        var appFolder = Path.Combine(appDataPath, "GabrielOS");
        Directory.CreateDirectory(appFolder);
        return Path.Combine(appFolder, "gabrielos.db");
    }

    protected override void OnExit(ExitEventArgs e)
    {
        _serviceProvider.Dispose();
        base.OnExit(e);
    }
}
