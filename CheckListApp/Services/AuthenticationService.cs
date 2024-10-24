using System;
using System.Threading.Tasks;
using CheckListApp.Model;
using CheckListApp.Repository;
using CheckListApp.Respository;

namespace CheckListApp.Services
{
    public interface IAuthenticationService
    {
        bool IsAuthenticated { get; }
        string CurrentUser { get; }
        Task<(bool success, string message)> LoginAsync(string username, string password);
        Task<(bool success, string message)> RegisterAsync(string username, string password, string email, string firstName = "", string lastName = "");
        Task<(bool success, string message)> ChangePasswordAsync(string username, string currentPassword, string newPassword);
        Task<bool> ValidateCredentialsAsync(string username, string password);
        void Logout();
    }

    public class AuthenticationService : IAuthenticationService
    {
        private bool _isAuthenticated = false;
        private string _currentUser = string.Empty;
        private readonly IUserRepository _userRepository;
        private readonly IPasswordHasher _passwordHasher;

        public bool IsAuthenticated => _isAuthenticated;
        public string CurrentUser => _currentUser;

        public AuthenticationService(IUserRepository userRepository, IPasswordHasher passwordHasher)
        {
            _userRepository = userRepository ?? throw new ArgumentNullException(nameof(userRepository));
            _passwordHasher = passwordHasher ?? throw new ArgumentNullException(nameof(passwordHasher));
        }

        public async Task<(bool success, string message)> LoginAsync(string username, string password)
        {
            try
            {
                if (string.IsNullOrWhiteSpace(username) || string.IsNullOrWhiteSpace(password))
                {
                    return (false, "Username and password are required.");
                }

                var user = await _userRepository.GetUserByUsernameAsync(username);
                if (user == null)
                {
                    return (false, "Invalid username or password.");
                }

                if (string.IsNullOrEmpty(user.PasswordHash))
                {
                    return (false, "Account requires password reset.");
                }

                if (_passwordHasher.VerifyPassword(password, user.PasswordHash))
                {
                    _isAuthenticated = true;
                    _currentUser = username;
                    return (true, "Login successful.");
                }

                return (false, "Invalid username or password.");
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"Login error: {ex.Message}");
                return (false, "An error occurred during login. Please try again.");
            }
        }

        public async Task<(bool success, string message)> RegisterAsync(
            string username,
            string password,
            string email,
            string firstName = "",
            string lastName = "")
        {
            try
            {
                // Input validation
                if (string.IsNullOrWhiteSpace(username) || string.IsNullOrWhiteSpace(password))
                {
                    return (false, "Username and password are required.");
                }

                if (string.IsNullOrWhiteSpace(email))
                {
                    return (false, "Email is required.");
                }

                if (!IsValidEmail(email))
                {
                    return (false, "Please enter a valid email address.");
                }

                if (!IsValidPassword(password))
                {
                    return (false, "Password must be at least 8 characters long and contain at least one uppercase letter, one lowercase letter, one number, and one special character.");
                }

                // Check if username already exists
                var existingUser = await _userRepository.GetUserByUsernameAsync(username);
                if (existingUser != null)
                {
                    return (false, "Username already exists.");
                }

                // Check if email is already registered
                var existingEmail = await _userRepository.GetUserByEmailAsync(email);
                if (existingEmail != null)
                {
                    return (false, "Email address is already registered.");
                }

                // Create new user
                var newUser = new Users
                {
                    Username = username,
                    Email = email,
                    PasswordHash = _passwordHasher.HashPassword(password),
                    FirstName = firstName,
                    LastName = lastName,
                    CreatedDate = DateTime.UtcNow,
                    UpdatedDate = DateTime.UtcNow
                };

                // Save user to database
                await _userRepository.AddAsync(newUser);
                return (true, "Registration successful.");
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"Registration error: {ex.Message}");
                return (false, "An error occurred during registration. Please try again.");
            }
        }

        public async Task<(bool success, string message)> ChangePasswordAsync(
            string username,
            string currentPassword,
            string newPassword)
        {
            try
            {
                var user = await _userRepository.GetUserByUsernameAsync(username);
                if (user == null)
                {
                    return (false, "User not found.");
                }

                // Verify current password
                if (!_passwordHasher.VerifyPassword(currentPassword, user.PasswordHash))
                {
                    return (false, "Current password is incorrect.");
                }

                // Validate new password
                if (!IsValidPassword(newPassword))
                {
                    return (false, "New password must be at least 8 characters long and contain at least one uppercase letter, one lowercase letter, one number, and one special character.");
                }

                // Update password
                user.PasswordHash = _passwordHasher.HashPassword(newPassword);
                user.UpdatedDate = DateTime.UtcNow;

                await _userRepository.UpdateAsync(user);
                return (true, "Password changed successfully.");
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"Change password error: {ex.Message}");
                return (false, "An error occurred while changing password. Please try again.");
            }
        }

        public async Task<bool> ValidateCredentialsAsync(string username, string password)
        {
            try
            {
                var user = await _userRepository.GetUserByUsernameAsync(username);
                if (user == null || string.IsNullOrEmpty(user.PasswordHash))
                {
                    return false;
                }

                return _passwordHasher.VerifyPassword(password, user.PasswordHash);
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"Validate credentials error: {ex.Message}");
                return false;
            }
        }

        public void Logout()
        {
            _isAuthenticated = false;
            _currentUser = string.Empty;
        }

<<<<<<< HEAD
<<<<<<< HEAD
=======
        public async Task<bool> RegisterAsync(string username, string password)
=======
        private bool IsValidEmail(string email)
>>>>>>> a4dd31a68ba90ee67b6f9cf9258f8834a5ee172c
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

        private bool IsValidPassword(string password)
        {
            if (string.IsNullOrWhiteSpace(password))
                return false;

            // Password must be at least 8 characters long and contain:
            // - at least one uppercase letter
            // - at least one lowercase letter
            // - at least one number
            // - at least one special character
            var hasNumber = password.Any(char.IsDigit);
            var hasUppercase = password.Any(char.IsUpper);
            var hasLowercase = password.Any(char.IsLower);
            var hasSpecialChar = password.Any(c => !char.IsLetterOrDigit(c));
            var isLongEnough = password.Length >= 8;

            return hasNumber && hasUppercase && hasLowercase && hasSpecialChar && isLongEnough;
        }
>>>>>>> d168323a227eead67c1e8097604120c73eabfc04
    }
}