using CheckListApp.ViewModels;
using CheckListApp.Services;
using CheckListApp.Respository;

namespace CheckListApp.View;

public partial class RegistrationPage : ContentPage
{
    private readonly RegistrationViewModel _viewModel;
    private readonly AuthenticationService _authService;

    public RegistrationPage(AuthenticationService authService)
    {
        InitializeComponent();
        _authService = authService;
        _viewModel = new RegistrationViewModel(_authService);
        BindingContext = _viewModel;
        _viewModel.RegistrationSuccessful += OnRegistrationSuccessful;
    }

    private async void OnRegistrationSuccessful(object sender, EventArgs e)
    {
        await Navigation.PushAsync(new LoginPage(new LoginViewModel(_authService)));
    }

    protected override void OnDisappearing()
    {
        base.OnDisappearing();
        _viewModel.RegistrationSuccessful -= OnRegistrationSuccessful;
    }
}