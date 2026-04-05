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
using GabrielOS.Presentation.Services;
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

        DispatcherUnhandledException += (_, ex) =>
        {
            MessageBox.Show($"Unexpected error: {ex.Exception.Message}", "Error",
                MessageBoxButton.OK, MessageBoxImage.Error);
            ex.Handled = true;
        };

        var splash = new SplashWindow();
        splash.Show();

        var services = new ServiceCollection();
        ConfigureServices(services);
        _serviceProvider = services.BuildServiceProvider();

        // Apply saved theme (or default Dark)
        var themeService = _serviceProvider.GetRequiredService<ThemeService>();
        themeService.LoadSavedTheme();

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
        services.AddScoped<ICalendarEventRepository, CalendarEventRepository>();

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
        services.AddScoped<CalendarService>();
        services.AddScoped<AIContextBuilder>();

        // Singleton: settings and theme live outside DI scope
        services.AddSingleton<SettingsService>();
        services.AddSingleton<ThemeService>();

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
        services.AddTransient<CalendarViewModel>();
        services.AddTransient<SettingsViewModel>();

        // Windows
        services.AddTransient<MainWindow>();
    }

    private async Task InitializeDatabaseAsync()
    {
        using var scope = _serviceProvider.CreateScope();
        var context = scope.ServiceProvider.GetRequiredService<AppDbContext>();
        try
        {
            await context.Database.MigrateAsync();
        }
        catch
        {
            // DB exists but was created without migrations — ensure schema is up to date.
            // EnsureCreated does nothing if the DB file already exists, so we apply
            // missing tables via raw SQL for each table the model expects.
            await context.Database.EnsureCreatedAsync();
            await EnsureMissingTablesAsync(context);
        }
        await DefaultDataSeeder.SeedAsync(context);
    }

    private static async Task EnsureMissingTablesAsync(AppDbContext context)
    {
        // Check for tables added after the initial EnsureCreated and create them if missing.
        var conn = context.Database.GetDbConnection();
        await conn.OpenAsync();
        try
        {
            using var cmd = conn.CreateCommand();
            cmd.CommandText = "SELECT name FROM sqlite_master WHERE type='table' AND name='CalendarEvents'";
            var exists = await cmd.ExecuteScalarAsync();
            if (exists == null)
            {
                using var create = conn.CreateCommand();
                create.CommandText = """
                    CREATE TABLE "CalendarEvents" (
                        "Id" TEXT NOT NULL CONSTRAINT "PK_CalendarEvents" PRIMARY KEY,
                        "UserId" TEXT NOT NULL,
                        "Title" TEXT NOT NULL,
                        "Description" TEXT,
                        "EventType" TEXT NOT NULL DEFAULT 'Other',
                        "Date" TEXT NOT NULL,
                        "EndDate" TEXT,
                        "StartTime" TEXT,
                        "EndTime" TEXT,
                        "IsAllDay" INTEGER NOT NULL DEFAULT 1,
                        "Location" TEXT,
                        "Priority" TEXT,
                        "IsCompleted" INTEGER NOT NULL DEFAULT 0,
                        "Notes" TEXT,
                        "GoalId" TEXT,
                        "PillarId" TEXT,
                        "Recurrence" TEXT NOT NULL DEFAULT 'None',
                        "RecurrenceEndDate" TEXT,
                        "CreatedAt" TEXT NOT NULL,
                        "UpdatedAt" TEXT NOT NULL,
                        CONSTRAINT "FK_CalendarEvents_Goals_GoalId" FOREIGN KEY ("GoalId") REFERENCES "Goals" ("Id"),
                        CONSTRAINT "FK_CalendarEvents_Pillars_PillarId" FOREIGN KEY ("PillarId") REFERENCES "Pillars" ("Id"),
                        CONSTRAINT "FK_CalendarEvents_Users_UserId" FOREIGN KEY ("UserId") REFERENCES "Users" ("Id") ON DELETE CASCADE
                    );
                    CREATE INDEX "IX_CalendarEvents_UserId_Date" ON "CalendarEvents" ("UserId", "Date");
                    CREATE INDEX "IX_CalendarEvents_UserId_EventType" ON "CalendarEvents" ("UserId", "EventType");
                    CREATE INDEX "IX_CalendarEvents_GoalId" ON "CalendarEvents" ("GoalId");
                    CREATE INDEX "IX_CalendarEvents_PillarId" ON "CalendarEvents" ("PillarId");
                    """;
                await create.ExecuteNonQueryAsync();
            }
        }
        finally
        {
            await conn.CloseAsync();
        }
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
