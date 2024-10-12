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
}