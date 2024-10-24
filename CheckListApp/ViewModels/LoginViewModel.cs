using CheckListApp.Services;
using CommunityToolkit.Mvvm.Input;
using System.Windows.Input;
using System.Threading.Tasks;

namespace CheckListApp.ViewModels
{
    public class LoginViewModel : BindableObject
    {
        private readonly AuthenticationService _authService;
        private string _username;
        private string _password;

        public string Username
        {
            get => _username;
            set
            {
                _username = value;
                OnPropertyChanged();
            }
        }

        public string Password
        {
            get => _password;
            set
            {
                _password = value;
                OnPropertyChanged();
            }
        }

        public ICommand LoginCommand { get; }

        public LoginViewModel(AuthenticationService authService)
        {
            _authService = authService;
            LoginCommand = new AsyncRelayCommand(OnLoginClicked);
        }

        private async Task OnLoginClicked()
        {
            if (string.IsNullOrWhiteSpace(Username) || string.IsNullOrWhiteSpace(Password))
            {
                await Application.Current.MainPage.DisplayAlert("Error", "Username and password are required.", "OK");
                return;
            }

        //    if (await _authService.LoginAsync(Username, Password))
        //    {
        //        // Create and set the AppShell as the MainPage if it doesn't exist
        //        if (Application.Current.MainPage is not AppShell)
        //        {
        //            Application.Current.MainPage = new AppShell();
        //        }

        //        // Now that we've ensured AppShell is set, we can safely use Shell.Current
        //        await Shell.Current.GoToAsync("//TaskEntryPage");
        //    }
        //    else
        //    {
        //        await Application.Current.MainPage.DisplayAlert("Error", "Invalid username or password.", "OK");
        //    }
        }
    }
}