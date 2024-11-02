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

    public App(IServiceProvider serviceProvider, IAuthenticationService authService)
    {
        InitializeComponent();

        _serviceProvider = serviceProvider;
        _authService = authService;

        //MainPage = new AppShell(_serviceProvider);
        MainPage = _serviceProvider.GetRequiredService<CustomSplashPage>();

        Task.Run(async () =>
        {
            var taskDatabase = _serviceProvider.GetRequiredService<TaskDatabase>();
            await taskDatabase.InitializeDatabaseAsync();
            //await taskDatabase.ExecuteAsync("DELETE FROM Users");
            //await taskDatabase.ExecuteAsync("DELETE FROM UserTask");
            Debug.WriteLine("Successfully cleared UserTasks table");
            //Debug.WriteLine("Successfully cleared Users table");

            // Display users in message box
            await DisplayUsersInMessageBox();

            // CHANGE THIS TO ROUTE TO LOGIN, IF USER HAS REGISTRED IF NOT REG PAGE
            await MainThread.InvokeOnMainThreadAsync(() =>
            {
                if (!_authService.IsAuthenticated)
                {
                    Shell.Current?.GoToAsync("//UserTaskPage");
                }
                else
                {
                    Shell.Current?.GoToAsync("//UserTaskPage");
                }
            });
        });
    }

    private async Task DisplayUsersInMessageBox()
    {
        try
        {
            var userService = _serviceProvider.GetRequiredService<UserService>();
            var users = await userService.GetAllUsersAsync();

            string userList = "Users in Database:\n\n";

            if (users != null && users.Any())
            {
                foreach (var user in users)
                {
                    userList += $"ID: {user.UserID}\n";
                    userList += $"Username: {user.Username}\n";
                    userList += $"Email: {user.Email}\n";
                    userList += $"First Name: {user.FirstName}\n";
                    userList += $"Last Name: {user.LastName}\n";
                    userList += $"Created: {user.CreatedDate}\n";
                    userList += "---------------\n";
                }
            }
            else
            {
                userList = "No users found in the database.";
            }

            await MainThread.InvokeOnMainThreadAsync(async () =>
            {
                await Application.Current.MainPage.DisplayAlert("Database Users", userList, "OK");
            });
        }
        catch (Exception ex)
        {
            await MainThread.InvokeOnMainThreadAsync(async () =>
            {
                await Application.Current.MainPage.DisplayAlert("Error",
                    $"Error retrieving users: {ex.Message}", "OK");
            });
        }
    }

    //public async Task RunDatabaseTests()
    //{
    //    try
    //    {
    //        // Initialize the database
    //        var taskDatabase = _serviceProvider.GetRequiredService<TaskDatabase>();
    //        await taskDatabase.InitializeDatabaseAsync();

    //        // Create and run the tests
    //        var testRepositories = new TestRepositories(
    //            (UserRepository)_serviceProvider.GetRequiredService<IUserRepository>(),  // Use interface
    //            _serviceProvider.GetRequiredService<UserTaskRepository>(),
    //            _serviceProvider.GetRequiredService<CommentRepository>(),
    //            _serviceProvider.GetRequiredService<NotificationRepository>()
    //        );
    //        await testRepositories.RunAllTests();

    //        Debug.WriteLine("All tests completed successfully.");
    //    }
    //    catch (Exception ex)
    //    {
    //        Debug.WriteLine($"Error running tests: {ex.Message}");
    //    }
    //}
}