using Microsoft.Extensions.Logging;
using Microsoft.Extensions.DependencyInjection;
using CheckListApp.Services;
using CheckListApp.ViewModels;
using CheckListApp.View;
using CheckListApp.Data;
using CommunityToolkit.Maui;

namespace CheckListApp;

public static class MauiProgram
{
    public static MauiApp CreateMauiApp()
    {
        var builder = MauiApp.CreateBuilder();
        builder
            .UseMauiApp<App>()
            .ConfigureFonts(fonts =>
            {
                fonts.AddFont("OpenSans-Regular.ttf", "OpenSansRegular");
                fonts.AddFont("OpenSans-Semibold.ttf", "OpenSansSemibold");
            })
            .UseMauiCommunityToolkit();

        builder.Logging.AddConfiguration(builder.Configuration.GetSection("Logging"));

        builder.Services.AddSingleton<TaskDatabase>();

        builder.Services.AddSingleton<UserTaskService>();
        builder.Services.AddSingleton<AuthenticationService>();
        builder.Services.AddSingleton<UserService>();

        builder.Services.AddTransient<UserTaskViewModel>();
        //builder.Services.AddTransient<__MainPageViewModel>();
        builder.Services.AddTransient<LoginViewModel>();
        builder.Services.AddTransient<TaskEntryViewModel>();

        builder.Services.AddTransient<MainPage>();
        builder.Services.AddTransient<UserTaskPage>();
        builder.Services.AddTransient<ItemDetailPage>();
        builder.Services.AddTransient<LoginPage>();
        builder.Services.AddTransient<TaskEntryPage>();

        builder.Services.AddSingleton<App>();

        return builder.Build();
    }
}