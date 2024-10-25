using CheckListApp.View;
using Microsoft.Maui;
using Microsoft.Extensions.DependencyInjection;
using CheckListApp.Respository;
using System.Threading.Tasks;

namespace CheckListApp
{
    public partial class AppShell : Shell
    {
        //private readonly TestRepositories _testRepositories;
        private readonly IServiceProvider _serviceProvider;

        public AppShell()
        {
            InitializeComponent();
            RegisterRoutes();
        }

        public AppShell(IServiceProvider serviceProvider) : this()
        {
            _serviceProvider = serviceProvider;

            // Initialize TestRepositories with the required repositories
            //_testRepositories = new TestRepositories(
            //    serviceProvider.GetRequiredService<UserRepository>(),
            //    serviceProvider.GetRequiredService<UserTaskRepository>(),
            //    serviceProvider.GetRequiredService<CommentRepository>(),
            //    serviceProvider.GetRequiredService<NotificationRepository>()
            //);
        }

        private void RegisterRoutes()
        {
            Routing.RegisterRoute(nameof(UserTaskPage), typeof(UserTaskPage));
            Routing.RegisterRoute(nameof(MainPage), typeof(MainPage));
            Routing.RegisterRoute(nameof(LoginPage), typeof(LoginPage));
            Routing.RegisterRoute(nameof(TaskEntryPage), typeof(TaskEntryPage));
            Routing.RegisterRoute(nameof(RegistrationPage), typeof(RegistrationPage));
            Routing.RegisterRoute(nameof(CustomSplashPage), typeof(CustomSplashPage));

            // Add default content
            this.Items.Add(new ShellContent
            {
                Route = "main",
                ContentTemplate = new DataTemplate(() =>
                    _serviceProvider?.GetService<MainPage>() ?? new MainPage())
            });
        }

        public async Task NavigateToMainPage()
        {
            await Shell.Current.GoToAsync("//main");
        }

        //public async Task RunAllTests()
        //{
        //    if (_testRepositories != null)
        //    {
        //        await _testRepositories.RunAllTests();
        //    }
        //    else
        //    {
        //        Console.WriteLine("TestRepositories is not initialized. Unable to run tests.");
        //    }
        //}

        public async Task NavigateToLoginPage()
        {
            await Shell.Current.GoToAsync(nameof(LoginPage));
        }

        public async Task NavigateToRegistrationPage()
        {
            await Shell.Current.GoToAsync(nameof(RegistrationPage));
        }
    }
}