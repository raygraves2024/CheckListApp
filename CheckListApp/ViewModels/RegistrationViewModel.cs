using System;
using System.Threading.Tasks;
using System.Windows.Input;
using System.Text;
using CheckListApp.Services;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using CommunityToolkit.Mvvm.Input;

namespace CheckListApp.ViewModels
{

    public class RegistrationViewModel : INotifyPropertyChanged
    {
        private readonly AuthenticationService _authService;
        private string _username;
        private string _password;
        private string _statusMessage;

        public string Username
        {
            get => _username;
            set
            {
                if (_username != value)
                {
                    _username = value;
                    OnPropertyChanged();
                }
            }
        }
        public RegistrationViewModel(AuthenticationService authService)
        {
            _authService = authService;
            RegisterCommand = new AsyncRelayCommand(RegisterUser);
        }

        public string Password
        {
            get => _password;
            set
            {
                if (_password != value)
                {
                    _password = value;
                    OnPropertyChanged();
                }
            }
        }

        public string StatusMessage
        {
            get => _statusMessage;
            set
            {
                if (_statusMessage != value)
                {
                    _statusMessage = value;
                    OnPropertyChanged();
                }
            }
        }

        public ICommand RegisterCommand { get; private set; }

        public event EventHandler RegistrationSuccessful;

        private async Task RegisterUser()
        {
            if (string.IsNullOrWhiteSpace(Username) || string.IsNullOrWhiteSpace(Password))
            {
                StatusMessage = "Please enter both username and password.";
                return;
            }

            try
            {
                bool isRegistered = await _authService.RegisterAsync(Username, Password);

                if (isRegistered)
                {
                    StatusMessage = "Registration successful!";
                    RegistrationSuccessful?.Invoke(this, EventArgs.Empty);
                }
                else
                {
                    StatusMessage = "Registration failed. Username may already exist.";
                }

                // Clear the input fields after registration attempt
                Username = string.Empty;
                Password = string.Empty;
            }
            catch (Exception ex)
            {
                StatusMessage = $"Registration failed: {ex.Message}";
            }
        }

        public event PropertyChangedEventHandler PropertyChanged;

        protected virtual void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}