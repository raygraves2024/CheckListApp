using CheckListApp.Services;
using CheckListApp.View;
using Microsoft.Extensions.DependencyInjection;

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
            MainPage = new NavigationPage(_serviceProvider.GetRequiredService<LoginPage>());
        }
        else
        {
            MainPage = new AppShell();
            Shell.Current.GoToAsync("//TaskEntryPage");
        }
    }
}