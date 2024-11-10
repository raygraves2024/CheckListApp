using CheckListApp.Services;
using CommunityToolkit.Mvvm.Input;
using System.Windows.Input;
using System.Threading.Tasks;

namespace CheckListApp.ViewModels
{
    public class LoginViewModel : BindableObject
    {
        private readonly AuthenticationService _authService;
        private readonly IPasswordHasher _passwordHasher;
        private string _username;
        private string _password;
        private bool _isLoading;
        private string _errorMessage;

        public string Username
        {
            get => _username;
            set
            {
                _username = value;
                OnPropertyChanged();
                ClearErrorMessage();
            }
        }

        public string Password
        {
            get => _password;
            set
            {
                _password = value;
                OnPropertyChanged();
                ClearErrorMessage();
            }
        }

        public bool IsLoading
        {
            get => _isLoading;
            set
            {
                _isLoading = value;
                OnPropertyChanged();
                ((AsyncRelayCommand)LoginCommand).NotifyCanExecuteChanged();
            }
        }

        public string ErrorMessage
        {
            get => _errorMessage;
            set
            {
                _errorMessage = value;
                OnPropertyChanged();
            }
        }

        public ICommand LoginCommand { get; }
        public ICommand NavigateToRegisterCommand { get; }

        public LoginViewModel(AuthenticationService authService, IPasswordHasher passwordHasher)
        {
            _authService = authService ?? throw new ArgumentNullException(nameof(authService));
            _passwordHasher = passwordHasher ?? throw new ArgumentNullException(nameof(passwordHasher));

            LoginCommand = new AsyncRelayCommand(OnLoginClicked, CanExecuteLogin);
            NavigateToRegisterCommand = new AsyncRelayCommand(OnNavigateToRegisterClicked);
        }

        public void ResetState()
        {
            Username = string.Empty;
            Password = string.Empty;
            ErrorMessage = string.Empty;
            IsLoading = false;
        }

        private bool CanExecuteLogin()
        {
            return !IsLoading &&
                   !string.IsNullOrWhiteSpace(Username) &&
                   !string.IsNullOrWhiteSpace(Password);
        }

        private void ClearErrorMessage()
        {
            ErrorMessage = string.Empty;
            ((AsyncRelayCommand)LoginCommand).NotifyCanExecuteChanged();
        }

        private async Task OnNavigateToRegisterClicked()
        {
            try
            {
                // Clear any existing error messages and sensitive data
                ResetState();

                // Force a new instance of RegistrationPage
                await Shell.Current.GoToAsync("//RegistrationPage", true);
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"Navigation error: {ex.Message}");
                await Application.Current.MainPage.DisplayAlert("Error",
                    "Unable to navigate to registration page. Please try again.", "OK");
            }
        }

        private async Task OnLoginClicked()
        {
            try
            {
                IsLoading = true;
                ErrorMessage = string.Empty;

                if (string.IsNullOrWhiteSpace(Username) || string.IsNullOrWhiteSpace(Password))
                {
                    ErrorMessage = "Username and password are required.";
                    return;
                }

                // Validate password complexity before attempting login
                if (Password.Length < 8)
                {
                    ErrorMessage = "Password must be at least 8 characters long.";
                    return;
                }

                var (success, message) = await _authService.LoginAsync(Username, Password);

                if (success)
                {
                    // Clear sensitive data
                    Password = string.Empty;

                    // Create and set the AppShell as the MainPage if it doesn't exist
                    if (Application.Current.MainPage is not AppShell)
                    {
                        Application.Current.MainPage = new AppShell();
                    }

                    // Navigate to the TaskEntryPage with a new navigation stack
                    await Shell.Current.GoToAsync("//UserTaskPage", true);
                }
                else
                {
                    ErrorMessage = message;
                    await Application.Current.MainPage.DisplayAlert("Login Failed", message, "OK");
                }
            }
            catch (Exception ex)
            {
                ErrorMessage = "An unexpected error occurred. Please try again.";
                await Application.Current.MainPage.DisplayAlert("Error",
                    "An unexpected error occurred. Please try again.", "OK");
                System.Diagnostics.Debug.WriteLine($"Login error: {ex.Message}");
            }
            finally
            {
                IsLoading = false;
            }
        }
    }
}