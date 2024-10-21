using System;
using System.Threading.Tasks;
using CheckListApp.Model;
using CheckListApp.Respository;

namespace CheckListApp.Services
{
    public class AuthenticationService
    {
        private bool _isAuthenticated = false;
        private string _currentUser = string.Empty;
        private readonly UserRepository _userRepository;

        public bool IsAuthenticated => _isAuthenticated;
        public string CurrentUser => _currentUser;

        public AuthenticationService(UserRepository userRepository)
        {
            _userRepository = userRepository;
        }

        public async Task<bool> LoginAsync(string username, string password)
        {
            // Implement actual authentication logic here
            var user = await _userRepository.GetUserByUsernameAsync(username);
            if (user != null && user.Password == password) // In a real app, use proper password hashing
            {
                _isAuthenticated = true;
                _currentUser = username;
                return true;
            }

            return false;
        }

        public void Logout()
        {
            _isAuthenticated = false;
            _currentUser = string.Empty;
        }

        public async Task<bool> RegisterAsync(string username, string password)
        {
            if (string.IsNullOrWhiteSpace(username) || string.IsNullOrWhiteSpace(password))
            {
                return false;
            }

            var existingUser = await _userRepository.GetUserByUsernameAsync(username);
            if (existingUser != null)
            {
                return false; // User already exists
            }

            var newUser = new Users
            {
                Username = username,
                Password = password // Note: In a real app, you should hash the password
            };

            await _userRepository.AddAsync(newUser);
            return true;
        }
    }
}