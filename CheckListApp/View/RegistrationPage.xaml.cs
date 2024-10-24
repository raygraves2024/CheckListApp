using CheckListApp.ViewModels;
using CheckListApp.Services;

namespace CheckListApp.View;

public partial class RegistrationPage : ContentPage
{
    private readonly RegistrationViewModel _viewModel;

    public RegistrationPage(IAuthenticationService authService, IPasswordHasher passwordHasher)
    {
        InitializeComponent();
        _viewModel = new RegistrationViewModel(authService, passwordHasher);
        BindingContext = _viewModel;

        // Subscribe to events
        _viewModel.RegistrationSuccessful += OnRegistrationSuccessful;
        _viewModel.NavigateToLoginRequested += OnNavigateToLogin;
    }

    private async void OnRegistrationSuccessful(object sender, EventArgs e)
    {
        await Shell.Current.GoToAsync("//LoginPage");
    }

    private async void OnNavigateToLogin(object sender, EventArgs e)
    {
        await Shell.Current.GoToAsync("//LoginPage");
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