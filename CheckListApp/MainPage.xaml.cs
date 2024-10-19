using CheckListApp.ViewModels;

namespace CheckListApp
{
    public partial class MainPage : ContentPage

    {
        private readonly TestRepositories _testRepositories;
        public MainPage(MainPageViewModel viewModel, TestRepositories testRepositories)
        {
            InitializeComponent();
            _testRepositories = testRepositories;
            BindingContext = viewModel;
        }
        private void RunRepositoryTests()
        {
            // Call methods from TestRepository to execute them at startup
            _testRepositories.RunAllTests();
        }

    }
}
