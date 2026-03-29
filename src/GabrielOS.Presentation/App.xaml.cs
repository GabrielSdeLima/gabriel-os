using System.IO;
using System.Windows;
using GabrielOS.Application.Services;
using GabrielOS.Domain.Interfaces;
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

        var services = new ServiceCollection();
        ConfigureServices(services);
        _serviceProvider = services.BuildServiceProvider();

        // Ensure database is created and seeded
        await InitializeDatabaseAsync();

        // Show main window
        var mainWindow = _serviceProvider.GetRequiredService<MainWindow>();
        mainWindow.Show();
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

        // Application Services
        services.AddScoped<PillarService>();
        services.AddScoped<CheckInService>();
        services.AddScoped<JournalService>();

        // Navigation
        services.AddSingleton<INavigationService, NavigationService>();

        // ViewModels
        services.AddTransient<MainViewModel>();
        services.AddTransient<DashboardViewModel>();
        services.AddTransient<PillarListViewModel>();
        services.AddTransient<CheckInViewModel>();
        services.AddTransient<JournalListViewModel>();

        // Windows
        services.AddTransient<MainWindow>();
    }

    private async Task InitializeDatabaseAsync()
    {
        using var scope = _serviceProvider.CreateScope();
        var context = scope.ServiceProvider.GetRequiredService<AppDbContext>();

        // Create database and apply migrations
        await context.Database.EnsureCreatedAsync();

        // Seed default data
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
