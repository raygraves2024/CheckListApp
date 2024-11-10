using CheckListApp.Services;
using CheckListApp.View;
using Microsoft.Extensions.DependencyInjection;
using CheckListApp.Data;
using System.Diagnostics;
using CheckListApp.Repository;
using CheckListApp.Respository;
using CheckListApp.Model;

namespace CheckListApp;

public partial class App : Application
{
    private readonly IServiceProvider _serviceProvider;
    private readonly IAuthenticationService _authService;
    private const bool ENABLE_DATABASE_CLEANUP = true;

    public App(IServiceProvider serviceProvider, IAuthenticationService authService)
    {
        InitializeComponent();

        _serviceProvider = serviceProvider;
        _authService = authService;

        // Set MainPage to CustomSplashPage first
        MainPage = _serviceProvider.GetRequiredService<CustomSplashPage>();

        // Start initialization process
        Task.Run(async () =>
        {
            try
            {
                // Wait a bit for splash screen to show
                await Task.Delay(100);

                // Initialize database
                var taskDatabase = _serviceProvider.GetRequiredService<TaskDatabase>();
                await taskDatabase.InitializeDatabaseAsync();

                // Perform database cleanup if enabled
                if (ENABLE_DATABASE_CLEANUP)
                {
                    Debug.WriteLine("Starting database cleanup...");
                    var options = new DatabaseCleanupOptions
                    {
                        ClearUsers = false,  // Explicitly set to true
                        ClearTasks = false,
                        ClearComments = false,
                        ClearNotifications = false,
                        ResetSequences = false
                    };
                    Debug.WriteLine($"Options created - ClearUsers set to: {options.ClearUsers}");
                    await DatabaseCleanupUtility.ClearData(_serviceProvider, options);
                }

                // Check if any users exist in the database
                var userRepository = _serviceProvider.GetRequiredService<IUserRepository>();
                var users = await userRepository.GetAllAsync();

                // Wait for splash animations to complete (approximately)
                await Task.Delay(4000);

                await MainThread.InvokeOnMainThreadAsync(() =>
                {
                    // Set the main page to AppShell
                    if (Application.Current.MainPage is not AppShell)
                    {
                        Application.Current.MainPage = new AppShell(_serviceProvider);
                    }

                    // Navigate to appropriate page based on user existence
                    if (users != null && users.Any())
                    {
                        // Users exist, go to login page
                        Shell.Current?.GoToAsync("//LoginPage");
                    }
                    else
                    {
                        // No users exist, go to registration page
                        Shell.Current?.GoToAsync("//RegistrationPage");
                    }
                });
            }
            catch (Exception ex)
            {
                Debug.WriteLine($"Startup error: {ex.Message}");
                await MainThread.InvokeOnMainThreadAsync(async () =>
                {
                    await Application.Current.MainPage.DisplayAlert(
                        "Error",
                        $"Startup error: {ex.Message}",
                        "OK"
                    );
                });
            }
        });
    }
}