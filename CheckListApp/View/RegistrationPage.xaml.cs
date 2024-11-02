using CheckListApp.ViewModels;
using CheckListApp.Services;
using CheckListApp.Data;
using System.Diagnostics;

namespace CheckListApp.View;

public partial class RegistrationPage : ContentPage
{
    private readonly RegistrationViewModel _viewModel;
    private readonly TaskDatabase _taskDatabase;

    public RegistrationPage(IAuthenticationService authService, IPasswordHasher passwordHasher)
    {
        InitializeComponent();
        _viewModel = new RegistrationViewModel(authService, passwordHasher);
        BindingContext = _viewModel;

        // Get the TaskDatabase singleton instance
        _taskDatabase = TaskDatabase.Instance;

        // Subscribe to events
        _viewModel.RegistrationSuccessful += OnRegistrationSuccessful;
        _viewModel.NavigateToLoginRequested += OnNavigateToLogin;
    }

    private async void OnRegistrationSuccessful(object sender, EventArgs e)
    {
        try
        {
            // Initialize database if not already initialized
            await _taskDatabase.InitializeDatabaseAsync();

            // Clear the UserTask table
            await _taskDatabase.ExecuteAsync("DELETE FROM UserTask");
            Debug.WriteLine("Successfully cleared UserTasks table");

            // Navigate to the ItemDetailPage
            await Shell.Current.GoToAsync("/ItemDetailPage");
        }
        catch (Exception ex)
        {
            Debug.WriteLine($"Error during registration navigation: {ex.Message}");
            // Still navigate even if database operations fail
            await Shell.Current.GoToAsync("/ItemDetailPage");
        }
    }

    private async void OnNavigateToLogin(object sender, EventArgs e)
    {
        try
        {
            // Initialize database if not already initialized
            await _taskDatabase.InitializeDatabaseAsync();

            // Clear the UserTask table
            await _taskDatabase.ExecuteAsync("DELETE FROM UserTask");
            Debug.WriteLine("Successfully cleared UserTasks table");

            // Navigate to the ItemDetailPage
            await Shell.Current.GoToAsync("/ItemDetailPage");
        }
        catch (Exception ex)
        {
            Debug.WriteLine($"Error during login navigation: {ex.Message}");
            // Still navigate even if database operations fail
            await Shell.Current.GoToAsync("/ItemDetailPage");
        }
    }

    protected override void OnDisappearing()
    {
        base.OnDisappearing();
        // Unsubscribe from events
        _viewModel.RegistrationSuccessful -= OnRegistrationSuccessful;
        _viewModel.NavigateToLoginRequested -= OnNavigateToLogin;
    }

    protected override bool OnBackButtonPressed()
    {
        if (_viewModel.IsRegistering)
        {
            // Prevent back navigation while registration is in progress
            return true;
        }
        return base.OnBackButtonPressed();
    }
}