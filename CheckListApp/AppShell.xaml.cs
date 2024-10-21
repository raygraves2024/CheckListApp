using CheckListApp.View;
using Microsoft.Maui;
using Microsoft.Extensions.DependencyInjection;
using CheckListApp.Respository;
using System.Threading.Tasks;

namespace CheckListApp
{
    public partial class AppShell : Shell
    {
        private readonly TestRepositories _testRepositories;

        public AppShell()
        {
            InitializeComponent();

            Routing.RegisterRoute(nameof(UserTaskPage), typeof(UserTaskPage));
            Routing.RegisterRoute(nameof(MainPage), typeof(MainPage));
            Routing.RegisterRoute(nameof(LoginPage), typeof(LoginPage));
            Routing.RegisterRoute(nameof(TaskEntryPage), typeof(TaskEntryPage));
            Routing.RegisterRoute(nameof(RegistrationPage), typeof(RegistrationPage));

            // Add a default content to the Shell
            this.Items.Add(new ShellContent
            {
                Route = "main",
                ContentTemplate = new DataTemplate(() => new MainPage())
            });
        }

        public AppShell(IServiceProvider serviceProvider) : this()
        {
            // Initialize TestRepositories with the required repositories
            _testRepositories = new TestRepositories(
                serviceProvider.GetRequiredService<UserRepository>(),
                serviceProvider.GetRequiredService<UserTaskRepository>(),
                serviceProvider.GetRequiredService<CommentRepository>(),
                serviceProvider.GetRequiredService<NotificationRepository>()
            );
        }

        public async Task NavigateToMainPage()
        {
            await Shell.Current.GoToAsync("//main");
        }

        public async Task RunAllTests()
        {
            if (_testRepositories != null)
            {
                await _testRepositories.RunAllTests();
            }
            else
            {
                Console.WriteLine("TestRepositories is not initialized. Unable to run tests.");
            }
        }

        // Uncomment and modify these methods if needed in the future
        
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