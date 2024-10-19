using CheckListApp.View;
using Microsoft.Maui;
using Microsoft.Extensions.DependencyInjection;

namespace CheckListApp;
public partial class AppShell : Shell
{
    public AppShell()
    {
        InitializeComponent();

        Routing.RegisterRoute(nameof(UserTaskPage), typeof(UserTaskPage));
        Routing.RegisterRoute(nameof(MainPage), typeof(MainPage));
        Routing.RegisterRoute(nameof(LoginPage), typeof(LoginPage));
        Routing.RegisterRoute(nameof(TaskEntryPage), typeof(TaskEntryPage));
        Routing.RegisterRoute(nameof(RegistrationPage), typeof(RegistrationPage));
    }

    public async Task NavigateToMainPage()
    {
        var mainPage = (MainPage)App.Current.Handler.MauiContext.Services.GetService(typeof(MainPage));
        await Shell.Current.GoToAsync(nameof(MainPage));
    }

    public async Task NavigateToLoginPage()
    {
        await Shell.Current.GoToAsync(nameof(LoginPage));
    }
    // Add this method to navigate to the RegistrationPage
    public async Task NavigateToRegistrationPage()
    {
        await Shell.Current.GoToAsync(nameof(RegistrationPage));
    }
}