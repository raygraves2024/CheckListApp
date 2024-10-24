using System;
using System.Threading.Tasks;
using System.Windows.Input;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using CommunityToolkit.Mvvm.Input;
using System.Text.RegularExpressions;
using CheckListApp.Services;

namespace CheckListApp.ViewModels
{
    public class RegistrationViewModel : INotifyPropertyChanged
    {
        private readonly IAuthenticationService _authService;
        private readonly IPasswordHasher _passwordHasher;
        private string _username = string.Empty;
        private string _password = string.Empty;
        private string _confirmPassword = string.Empty;
        private string _email = string.Empty;
        private string _firstName = string.Empty;
        private string _lastName = string.Empty;
        private string _statusMessage = string.Empty;
        private bool _isRegistering;
        private string _statusMessageColor = "Gray";
        private string _registerButtonText = "Register";
        private string _passwordRequirements = string.Empty;
        private bool _isPasswordValid;
        private readonly Dictionary<string, bool> _passwordCriteria;

        // Field validation indicators
        private string _usernameIndicator = "*";
        private string _passwordIndicator = "*";
        private string _confirmPasswordIndicator = "*";
        private string _emailIndicator = "*";
        private string _firstNameIndicator = "*";
        private string _lastNameIndicator = "*";

        public RegistrationViewModel(IAuthenticationService authService, IPasswordHasher passwordHasher)
        {
            _authService = authService ?? throw new ArgumentNullException(nameof(authService));
            _passwordHasher = passwordHasher ?? throw new ArgumentNullException(nameof(passwordHasher));
            RegisterCommand = new AsyncRelayCommand(RegisterUser, CanRegister);
            NavigateToLoginCommand = new AsyncRelayCommand(ExecuteNavigateToLogin);

            _passwordCriteria = new Dictionary<string, bool>
            {
                { "MinLength", false },
                { "HasUppercase", false },
                { "HasLowercase", false },
                { "HasDigit", false },
                { "HasSpecialChar", false }
            };

            UpdatePasswordRequirements();
        }

        public string Username
        {
            get => _username;
            set
            {
                if (_username != value)
                {
                    _username = value;
                    UsernameIndicator = !string.IsNullOrWhiteSpace(value) ? "✓" : "*";
                    OnPropertyChanged();
                    OnPropertyChanged(nameof(UsernameIndicator));
                    (RegisterCommand as AsyncRelayCommand)?.NotifyCanExecuteChanged();
                }
            }
        }

        public string Password
        {
            get => _password;
            set
            {
                if (_password != value)
                {
                    _password = value;
                    ValidatePassword(value);
                    PasswordIndicator = _isPasswordValid ? "✓" : "*";
                    OnPropertyChanged();
                    OnPropertyChanged(nameof(PasswordIndicator));
                    OnPropertyChanged(nameof(PasswordRequirements));
                    ValidatePasswordMatch();
                    (RegisterCommand as AsyncRelayCommand)?.NotifyCanExecuteChanged();
                }
            }
        }

        public string ConfirmPassword
        {
            get => _confirmPassword;
            set
            {
                if (_confirmPassword != value)
                {
                    _confirmPassword = value;
                    ValidatePasswordMatch();
                    ConfirmPasswordIndicator = (!string.IsNullOrWhiteSpace(value) && value == Password) ? "✓" : "*";
                    OnPropertyChanged();
                    OnPropertyChanged(nameof(ConfirmPasswordIndicator));
                    (RegisterCommand as AsyncRelayCommand)?.NotifyCanExecuteChanged();
                }
            }
        }

        public string Email
        {
            get => _email;
            set
            {
                if (_email != value)
                {
                    _email = value;
                    EmailIndicator = IsValidEmail(value) ? "✓" : "*";
                    OnPropertyChanged();
                    OnPropertyChanged(nameof(EmailIndicator));
                    (RegisterCommand as AsyncRelayCommand)?.NotifyCanExecuteChanged();
                }
            }
        }

        public string FirstName
        {
            get => _firstName;
            set
            {
                if (_firstName != value)
                {
                    _firstName = value;
                    FirstNameIndicator = !string.IsNullOrWhiteSpace(value) ? "✓" : "*";
                    OnPropertyChanged();
                    OnPropertyChanged(nameof(FirstNameIndicator));
                    (RegisterCommand as AsyncRelayCommand)?.NotifyCanExecuteChanged();
                }
            }
        }

        public string LastName
        {
            get => _lastName;
            set
            {
                if (_lastName != value)
                {
                    _lastName = value;
                    LastNameIndicator = !string.IsNullOrWhiteSpace(value) ? "✓" : "*";
                    OnPropertyChanged();
                    OnPropertyChanged(nameof(LastNameIndicator));
                    (RegisterCommand as AsyncRelayCommand)?.NotifyCanExecuteChanged();
                }
            }
        }

        public string UsernameIndicator
        {
            get => _usernameIndicator;
            private set
            {
                if (_usernameIndicator != value)
                {
                    _usernameIndicator = value;
                    OnPropertyChanged();
                }
            }
        }

        public string PasswordIndicator
        {
            get => _passwordIndicator;
            private set
            {
                if (_passwordIndicator != value)
                {
                    _passwordIndicator = value;
                    OnPropertyChanged();
                }
            }
        }

        public string ConfirmPasswordIndicator
        {
            get => _confirmPasswordIndicator;
            private set
            {
                if (_confirmPasswordIndicator != value)
                {
                    _confirmPasswordIndicator = value;
                    OnPropertyChanged();
                }
            }
        }

        public string EmailIndicator
        {
            get => _emailIndicator;
            private set
            {
                if (_emailIndicator != value)
                {
                    _emailIndicator = value;
                    OnPropertyChanged();
                }
            }
        }

        public string FirstNameIndicator
        {
            get => _firstNameIndicator;
            private set
            {
                if (_firstNameIndicator != value)
                {
                    _firstNameIndicator = value;
                    OnPropertyChanged();
                }
            }
        }

        public string LastNameIndicator
        {
            get => _lastNameIndicator;
            private set
            {
                if (_lastNameIndicator != value)
                {
                    _lastNameIndicator = value;
                    OnPropertyChanged();
                }
            }
        }

        public string PasswordRequirements
        {
            get => _passwordRequirements;
            private set
            {
                if (_passwordRequirements != value)
                {
                    _passwordRequirements = value;
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
                    OnPropertyChanged(nameof(HasStatusMessage));
                }
            }
        }

        public string StatusMessageColor
        {
            get => _statusMessageColor;
            set
            {
                if (_statusMessageColor != value)
                {
                    _statusMessageColor = value;
                    OnPropertyChanged();
                }
            }
        }

        public string RegisterButtonText
        {
            get => _registerButtonText;
            set
            {
                if (_registerButtonText != value)
                {
                    _registerButtonText = value;
                    OnPropertyChanged();
                }
            }
        }

        public bool IsRegistering
        {
            get => _isRegistering;
            private set
            {
                if (_isRegistering != value)
                {
                    _isRegistering = value;
                    OnPropertyChanged();
                    OnPropertyChanged(nameof(IsNotRegistering));
                    (RegisterCommand as AsyncRelayCommand)?.NotifyCanExecuteChanged();
                }
            }
        }

        public bool IsNotRegistering => !IsRegistering;
        public bool HasStatusMessage => !string.IsNullOrEmpty(StatusMessage);

        public ICommand RegisterCommand { get; }
        public ICommand NavigateToLoginCommand { get; }

        public event EventHandler? RegistrationSuccessful;
        public event EventHandler? NavigateToLoginRequested;

        private bool CanRegister()
        {
            var canRegister = !IsRegistering &&
                   !string.IsNullOrWhiteSpace(Username) &&
                   !string.IsNullOrWhiteSpace(Password) &&
                   !string.IsNullOrWhiteSpace(ConfirmPassword) &&
                   !string.IsNullOrWhiteSpace(Email) &&
                   !string.IsNullOrWhiteSpace(FirstName) &&
                   !string.IsNullOrWhiteSpace(LastName) &&
                   Password == ConfirmPassword &&
                   IsValidEmail(Email) &&
                   _isPasswordValid;

            return canRegister;
        }

        private void ValidatePassword(string password)
        {
            _passwordCriteria["MinLength"] = password.Length >= 8;
            _passwordCriteria["HasUppercase"] = password.Any(char.IsUpper);
            _passwordCriteria["HasLowercase"] = password.Any(char.IsLower);
            _passwordCriteria["HasDigit"] = password.Any(char.IsDigit);
            _passwordCriteria["HasSpecialChar"] = password.Any(c => !char.IsLetterOrDigit(c));

            _isPasswordValid = _passwordCriteria.All(c => c.Value);

            UpdatePasswordRequirements();
        }

        private void ValidatePasswordMatch()
        {
            if (!string.IsNullOrEmpty(Password) && !string.IsNullOrEmpty(ConfirmPassword))
            {
                var passwordsMatch = Password == ConfirmPassword;
                if (!passwordsMatch)
                {
                    SetErrorStatus("Passwords do not match");
                }
                else if (_isPasswordValid)
                {
                    ClearStatus();
                }
            }
        }

        private void UpdatePasswordRequirements()
        {
            var requirements = new List<string>
            {
                $"{(_passwordCriteria["MinLength"] ? "✓" : "•")} At least 8 characters",
                $"{(_passwordCriteria["HasUppercase"] ? "✓" : "•")} One uppercase letter",
                $"{(_passwordCriteria["HasLowercase"] ? "✓" : "•")} One lowercase letter",
                $"{(_passwordCriteria["HasDigit"] ? "✓" : "•")} One number",
                $"{(_passwordCriteria["HasSpecialChar"] ? "✓" : "•")} One special character"
            };

            PasswordRequirements = string.Join("\n", requirements);
        }

        private bool IsValidEmail(string email)
        {
            try
            {
                var addr = new System.Net.Mail.MailAddress(email);
                return addr.Address == email;
            }
            catch
            {
                return false;
            }
        }

        private async Task RegisterUser()
        {
            if (IsRegistering) return;

            try
            {
                IsRegistering = true;
                RegisterButtonText = "Registering...";
                StatusMessage = "Processing registration...";
                StatusMessageColor = "Gray";

                if (!ValidateRegistrationInput())
                {
                    return;
                }

                var hashedPassword = _passwordHasher.HashPassword(Password);
                var (success, message) = await _authService.RegisterAsync(
                    Username,
                    hashedPassword,
                    Email,
                    FirstName,
                    LastName);

                if (success)
                {
                    SetSuccessStatus("Registration successful!");
                    ClearFields();
                    OnRegistrationSuccessful();
                }
                else
                {
                    SetErrorStatus(message);
                }
            }
            catch (Exception ex)
            {
                SetErrorStatus("An error occurred during registration. Please try again.");
            }
            finally
            {
                IsRegistering = false;
                RegisterButtonText = "Register";
            }
        }

        private bool ValidateRegistrationInput()
        {
            if (Password != ConfirmPassword)
            {
                SetErrorStatus("Passwords do not match.");
                return false;
            }

            if (!_isPasswordValid)
            {
                SetErrorStatus("Please ensure your password meets all requirements.");
                return false;
            }

            if (!IsValidEmail(Email))
            {
                SetErrorStatus("Please enter a valid email address.");
                return false;
            }

            return true;
        }

        private Task ExecuteNavigateToLogin()
        {
            OnNavigateToLoginRequested();
            return Task.CompletedTask;
        }

        private void SetErrorStatus(string message)
        {
            StatusMessage = message;
            StatusMessageColor = "Red";
        }

        private void SetSuccessStatus(string message)
        {
            StatusMessage = message;
            StatusMessageColor = "Green";
        }

        private void ClearStatus()
        {
            StatusMessage = string.Empty;
            StatusMessageColor = "Gray";
        }

        private void ClearFields()
        {
            Username = string.Empty;
            Password = string.Empty;
            ConfirmPassword = string.Empty;
            Email = string.Empty;
            FirstName = string.Empty;
            LastName = string.Empty;

            // Reset indicators
            UsernameIndicator = "*";
            PasswordIndicator = "*";
            ConfirmPasswordIndicator = "*";
            EmailIndicator = "*";
            FirstNameIndicator = "*";
            LastNameIndicator = "*";
        }

        protected virtual void OnRegistrationSuccessful()
        {
            RegistrationSuccessful?.Invoke(this, EventArgs.Empty);
        }

        protected virtual void OnNavigateToLoginRequested()
        {
            NavigateToLoginRequested?.Invoke(this, EventArgs.Empty);
        }

        public event PropertyChangedEventHandler? PropertyChanged;

        protected virtual void OnPropertyChanged([CallerMemberName] string? propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}