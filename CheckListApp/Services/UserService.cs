using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using CheckListApp.Model;
using CheckListApp.Data;
using SQLite;
using System.Linq;

namespace CheckListApp.Services
{
    public class UserService
    {
        private readonly TaskDatabase _database;
        private readonly IPasswordHasher _passwordHasher;

        public UserService(TaskDatabase database, IPasswordHasher passwordHasher)
        {
            _database = database ?? throw new ArgumentNullException(nameof(database));
            _passwordHasher = passwordHasher ?? throw new ArgumentNullException(nameof(passwordHasher));
        }

        public async Task UpdateTaskAsync(UserTask task)
        {
            await Task.Delay(100); // Placeholder for actual update operation
        }

        public async Task<Users> GetUserAsync(int userId)
        {
            await _database.InitializeDatabaseAsync();
            var table = await _database.Table<Users>();
            return await table.FirstOrDefaultAsync(u => u.UserID == userId);
        }

        public async Task<Users> GetUserByUsernameAsync(string username)
        {
            await _database.InitializeDatabaseAsync();
            var table = await _database.Table<Users>();
            return await table.FirstOrDefaultAsync(u => u.Username == username);
        }

        public async Task<Users> GetFirstUserAsync()
        {
            await _database.InitializeDatabaseAsync();
            var table = await _database.Table<Users>();
            return await table.FirstOrDefaultAsync();
        }

        public async Task<List<Users>> GetAllUsersAsync()
        {
            await _database.InitializeDatabaseAsync();
            var table = await _database.Table<Users>();
            return await table.ToListAsync();
        }

        public async Task<(bool success, string message)> SaveUserAsync(Users user, string password = null)
        {
            try
            {
                await _database.InitializeDatabaseAsync();

                // If this is a new user and password is provided, hash it
                if (user.UserID == 0 && !string.IsNullOrEmpty(password))
                {
                    user.PasswordHash = _passwordHasher.HashPassword(password);
                }

                if (user.UserID != 0)
                {
                    user.UpdatedDate = DateTime.UtcNow;
                    await _database.UpdateAsync(user);
                }
                else
                {
                    user.CreatedDate = DateTime.UtcNow;
                    user.UpdatedDate = DateTime.UtcNow;
                    await _database.InsertAsync(user);
                }
                return (true, "User saved successfully.");
            }
            catch (Exception ex)
            {
                return (false, $"Error saving user: {ex.Message}");
            }
        }

        public async Task<int> DeleteUserAsync(Users user)
        {
            await _database.InitializeDatabaseAsync();
            return await _database.DeleteAsync(user);
        }

        public async Task<(bool success, Users user, string message)> AuthenticateUserAsync(string username, string password)
        {
            try
            {
                var user = await GetUserByUsernameAsync(username);
                if (user == null)
                {
                    return (false, null, "Invalid username or password.");
                }

                if (string.IsNullOrEmpty(user.PasswordHash))
                {
                    return (false, null, "User account requires password reset.");
                }

                if (_passwordHasher.VerifyPassword(password, user.PasswordHash))
                {
                    return (true, user, "Authentication successful.");
                }

                return (false, null, "Invalid username or password.");
            }
            catch (Exception ex)
            {
                return (false, null, "An error occurred during authentication.");
            }
        }

        public async Task<(bool success, string message)> ChangePasswordAsync(int userId, string currentPassword, string newPassword)
        {
            try
            {
                var user = await GetUserAsync(userId);
                if (user == null)
                {
                    return (false, "User not found.");
                }

                // Verify current password
                if (!_passwordHasher.VerifyPassword(currentPassword, user.PasswordHash))
                {
                    return (false, "Current password is incorrect.");
                }

                // Hash and save new password
                user.PasswordHash = _passwordHasher.HashPassword(newPassword);
                user.UpdatedDate = DateTime.UtcNow;
                await _database.UpdateAsync(user);

                return (true, "Password changed successfully.");
            }
            catch (Exception ex)
            {
                return (false, "An error occurred while changing the password.");
            }
        }

        public async Task<(bool success, string message)> UpdateEmailAsync(int userId, string newEmail)
        {
            try
            {
                var user = await GetUserAsync(userId);
                if (user == null)
                {
                    return (false, "User not found.");
                }

                user.Email = newEmail;
                user.UpdatedDate = DateTime.UtcNow;
                await _database.UpdateAsync(user);
                return (true, "Email updated successfully.");
            }
            catch (Exception ex)
            {
                return (false, "An error occurred while updating the email.");
            }
        }

        public async Task<bool> IsUsernameTakenAsync(string username)
        {
            await _database.InitializeDatabaseAsync();
            var table = await _database.Table<Users>();
            var user = await table.FirstOrDefaultAsync(u => u.Username == username);
            return user != null;
        }

        public async Task<bool> IsEmailTakenAsync(string email)
        {
            await _database.InitializeDatabaseAsync();
            var table = await _database.Table<Users>();
            var user = await table.FirstOrDefaultAsync(u => u.Email == email);
            return user != null;
        }

        public async Task<int> GetUserCountAsync()
        {
            await _database.InitializeDatabaseAsync();
            var table = await _database.Table<Users>();
            return await table.CountAsync();
        }
    }
}