using System;
using System.Security.Cryptography;
using System.Threading.Tasks;
using System.Linq;
using CheckListApp.Model;
using CheckListApp.Data;

namespace CheckListApp.Services
{
    public class RegistrationService
    {
        private static RegistrationService _instance;
        private static readonly object _lock = new object();
        private readonly TaskDatabase _database;

        private RegistrationService()
        {
            _database = TaskDatabase.Instance;
        }

        public static RegistrationService Instance
        {
            get
            {
                if (_instance == null)
                {
                    lock (_lock)
                    {
                        _instance ??= new RegistrationService();
                    }
                }
                return _instance;
            }
        }

        public async Task<bool> RegisterUser(string username, string password, string email, string firstName, string lastName)
        {
            if (string.IsNullOrWhiteSpace(username) || string.IsNullOrWhiteSpace(password) || string.IsNullOrWhiteSpace(email))
            {
                throw new ArgumentException("Username, password, and email are required.");
            }

            var usersTable = await _database.Table<Users>();
            var existingUser = await usersTable.Where(u => u.Username == username).FirstOrDefaultAsync();
            if (existingUser != null)
            {
                throw new InvalidOperationException("Username already exists.");
            }

            byte[] passwordHash = GeneratePasswordHash(password);

            var newUser = new Users
            {
                Username = username,
                PasswordHash = passwordHash,
                Email = email,
                FirstName = firstName,
                LastName = lastName,
                CreatedDate = DateTime.UtcNow,
                UpdatedDate = DateTime.UtcNow
            };

            await _database.InsertAsync(newUser);
            return true;
        }

        private byte[] GeneratePasswordHash(string password)
        {
            using (var sha256 = SHA256.Create())
            {
                return sha256.ComputeHash(System.Text.Encoding.UTF8.GetBytes(password));
            }
        }
    }
}