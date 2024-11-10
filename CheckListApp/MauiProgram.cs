using Microsoft.Extensions.Logging;
using Microsoft.Extensions.DependencyInjection;
using CheckListApp.Services;
using CheckListApp.ViewModels;
using CheckListApp.View;
using CheckListApp.Data;
using CommunityToolkit.Maui;
using CheckListApp.Repository;
using CheckListApp.Converters;
using SQLite;
using System.IO;
using CheckListApp.Respository;
using System.Diagnostics;

namespace CheckListApp;

public static class MauiProgram
{
    public static MauiApp CreateMauiApp()
    {
        var builder = MauiApp.CreateBuilder();

        // Configure platform-specific handlers
        ConfigurePlatformHandlers(builder);

        // Configure basic MAUI settings
        ConfigureBasicSettings(builder);

        // Configure logging
        ConfigureLogging(builder);

        // Configure database
        ConfigureDatabase(builder);

        // Register repositories
        RegisterRepositories(builder.Services);

        // Register services
        RegisterServices(builder.Services);

        // Register viewmodels
        RegisterViewModels(builder.Services);

        // Register pages
        RegisterPages(builder.Services);

        return builder.Build();
    }

    private static void ConfigurePlatformHandlers(MauiAppBuilder builder)
    {
        Microsoft.Maui.Handlers.EntryHandler.Mapper.AppendToMapping("CursorColor", (handler, view) =>
        {
#if IOS
            handler.PlatformView.TintColor = UIKit.UIColor.Green;
#endif
        });
    }

    private static void ConfigureBasicSettings(MauiAppBuilder builder)
    {
        builder
            .UseMauiApp<App>()
            .ConfigureFonts(fonts =>
            {
                fonts.AddFont("OpenSans-Regular.ttf", "OpenSansRegular");
                fonts.AddFont("OpenSans-Semibold.ttf", "OpenSansSemibold");
            })
            .UseMauiCommunityToolkit();
    }

    private static void ConfigureLogging(MauiAppBuilder builder)
    {
#if DEBUG
        builder.Services.AddLogging(logging =>
        {
            logging.AddConsole();
            logging.SetMinimumLevel(LogLevel.Debug);
        });
#endif
    }

    private static void ConfigureDatabase(MauiAppBuilder builder)
    {
        builder.Services.AddSingleton<SQLiteAsyncConnection>(_ =>
        {
            var dbPath = Path.Combine(FileSystem.AppDataDirectory, "checklist.db3");
            return new SQLiteAsyncConnection(dbPath);
        });

        builder.Services.AddSingleton<TaskDatabase>();
    }

    private static void RegisterRepositories(IServiceCollection services)
    {
        // User Repository
        services.AddSingleton<UserRepository>();
        services.AddSingleton<IUserRepository>(sp =>
            sp.GetRequiredService<UserRepository>());

        // Task Repository
        services.AddSingleton<UserTaskRepository>();

        // Comment Repository
        services.AddSingleton<CommentRepository>();

        // Notification Repository
        services.AddSingleton<NotificationRepository>();
    }

    private static void RegisterServices(IServiceCollection services)
    {
        // Authentication related services
        services.AddSingleton<IPasswordHasher, PasswordHasher>();
        services.AddSingleton<AuthenticationService>();
        services.AddSingleton<IAuthenticationService>(sp =>
            sp.GetRequiredService<AuthenticationService>());

        // User related services
        services.AddSingleton<UserService>();
        services.AddSingleton<UserTaskService>();
    }

    private static void RegisterViewModels(IServiceCollection services)
    {
        services.AddTransient<UserTaskViewModel>();
        services.AddTransient<MainPageViewModel>();
        services.AddTransient<LoginViewModel>();
        services.AddTransient<TaskEntryViewModel>();
        services.AddTransient<RegistrationViewModel>();
    }

    private static void RegisterPages(IServiceCollection services)
    {
        // Shell
        services.AddSingleton<AppShell>();

        // Pages
        services.AddTransient<MainPage>();
        services.AddTransient<UserTaskPage>();
        services.AddTransient<ItemDetailPage>();
        services.AddTransient<LoginPage>();
        services.AddTransient<TaskEntryPage>();
        services.AddTransient<RegistrationPage>();
        services.AddTransient<CustomSplashPage>();
    }

#if DEBUG
    public static async Task RunDatabaseTests()
    {
        var app = CreateMauiApp();
        using var scope = app.Services.CreateScope();
        var database = scope.ServiceProvider.GetRequiredService<TaskDatabase>();

        try
        {
            await database.InitializeDatabaseAsync();
            Debug.WriteLine("Database tests completed successfully");
        }
        catch (Exception ex)
        {
            Debug.WriteLine($"Database test error: {ex.Message}");
            throw;
        }
    }

    public static async Task ClearDatabase()
    {
        var app = CreateMauiApp();
        using var scope = app.Services.CreateScope();

        try
        {
            await DatabaseCleanupUtility.ClearAllData(scope.ServiceProvider);
            Debug.WriteLine("Database cleared successfully");
        }
        catch (Exception ex)
        {
            Debug.WriteLine($"Database cleanup error: {ex.Message}");
            throw;
        }
    }

    public static async Task ResetAndInitializeDatabase()
    {
        var app = CreateMauiApp();
        using var scope = app.Services.CreateScope();

        try
        {
            // First clear all data
            await DatabaseCleanupUtility.ClearAllData(scope.ServiceProvider);

            // Then reinitialize the database
            var database = scope.ServiceProvider.GetRequiredService<TaskDatabase>();
            await database.InitializeDatabaseAsync();

            Debug.WriteLine("Database reset and initialization completed successfully");
        }
        catch (Exception ex)
        {
            Debug.WriteLine($"Database reset error: {ex.Message}");
            throw;
        }
    }
#endif
}