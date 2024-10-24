﻿using CheckListApp.Services;
using CheckListApp.View;
using Microsoft.Extensions.DependencyInjection;
using CheckListApp.Data;
using System.Diagnostics;
using CheckListApp.Repository;
using CheckListApp.Respository;

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

        // Set CustomSplashPage as the initial page
        MainPage = _serviceProvider.GetRequiredService<CustomSplashPage>();

        // Move the database initialization to the splash screen
        Task.Run(async () =>
        {
            var taskDatabase = _serviceProvider.GetRequiredService<TaskDatabase>();
            await taskDatabase.InitializeDatabaseAsync();
            await taskDatabase.ExecuteAsync("DELETE FROM Users");
        });
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