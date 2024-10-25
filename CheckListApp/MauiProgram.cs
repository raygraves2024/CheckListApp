using Microsoft.Extensions.Logging;
using Microsoft.Extensions.DependencyInjection;
using CheckListApp.Services;
using CheckListApp.ViewModels;
using CheckListApp.View;
using CheckListApp.Data;
using CommunityToolkit.Maui;
using CheckListApp.Repository;
using SQLite;
using System.IO;
using CheckListApp.Respository;
using CheckListApp.View;

namespace CheckListApp;

public static class MauiProgram
{
    public static MauiApp CreateMauiApp()
    {
        var builder = MauiApp.CreateBuilder();
        builder
            .UseMauiApp<App>()
            .ConfigureFonts(fonts =>
            {
                fonts.AddFont("OpenSans-Regular.ttf", "OpenSansRegular");
                fonts.AddFont("OpenSans-Semibold.ttf", "OpenSansSemibold");
            })
            .UseMauiCommunityToolkit();

        builder.Logging.AddConfiguration(builder.Configuration.GetSection("Logging"));

        // Database Connection
        builder.Services.AddSingleton<SQLiteAsyncConnection>(_ =>
            new SQLiteAsyncConnection(Path.Combine(FileSystem.AppDataDirectory, "checklist.db3")));
        builder.Services.AddSingleton<TaskDatabase>();

        // Register Repositories
        builder.Services.AddSingleton<UserRepository>();
        builder.Services.AddSingleton<IUserRepository>(sp => sp.GetRequiredService<UserRepository>());
        builder.Services.AddSingleton<UserTaskRepository>();
        builder.Services.AddSingleton<CommentRepository>();
        builder.Services.AddSingleton<NotificationRepository>();

        // Register Services
        builder.Services.AddSingleton<IPasswordHasher, PasswordHasher>();
        builder.Services.AddSingleton<AuthenticationService>();
        builder.Services.AddSingleton<IAuthenticationService>(sp => sp.GetRequiredService<AuthenticationService>());
        builder.Services.AddSingleton<UserService>();
        builder.Services.AddSingleton<UserTaskService>();

        // Register ViewModels
        builder.Services.AddTransient<UserTaskViewModel>();
        builder.Services.AddTransient<MainPageViewModel>();
        builder.Services.AddTransient<LoginViewModel>();
        builder.Services.AddTransient<TaskEntryViewModel>();
        builder.Services.AddTransient<RegistrationViewModel>();

        // Register Pages
        builder.Services.AddTransient<CustomSplashPage>();
        builder.Services.AddTransient<MainPage>();
        builder.Services.AddTransient<UserTaskPage>();
        builder.Services.AddTransient<ItemDetailPage>();
        builder.Services.AddTransient<LoginPage>();
        builder.Services.AddTransient<TaskEntryPage>();
        builder.Services.AddTransient<RegistrationPage>();
        builder.Services.AddSingleton<AppShell>();

        return builder.Build();
    }

    public static async Task RunDatabaseTests()
    {
        var app = CreateMauiApp();
        var appInstance = app.Services.GetRequiredService<App>();
        //await appInstance.RunDatabaseTests();
    }
}