using CheckListApp.Services;
using CheckListApp.View;
using Microsoft.Extensions.DependencyInjection;
using CheckListApp.Data;
using System.Diagnostics;

namespace CheckListApp;

public partial class App : Application
{
    private readonly IServiceProvider _serviceProvider;

    public App(IServiceProvider serviceProvider, AuthenticationService authService)
    {
        InitializeComponent();

        _serviceProvider = serviceProvider;
     

        if (!authService.IsAuthenticated)
        {
            MainPage = new AppShell();
            Shell.Current.GoToAsync("//RegistrationPage");
        }
        else
        {
            MainPage = new NavigationPage(_serviceProvider.GetRequiredService<LoginPage>());
        }
    }

    public async Task RunDatabaseTests()
    {
        try
        {
            // Initialize the database
            var taskDatabase = _serviceProvider.GetRequiredService<TaskDatabase>();
            await taskDatabase.InitializeDatabaseAsync();

            // Create and run the tests
            var testRepositories = new TestRepositories();
            await testRepositories.RunAllTests();

            Debug.WriteLine("All tests completed successfully.");
        }
        catch (Exception ex)
        {
            Debug.WriteLine($"Error running tests: {ex.Message}");
        }
    }
}