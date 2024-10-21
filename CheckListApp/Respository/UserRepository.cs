using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using CheckListApp.Model;
using SQLite;

namespace CheckListApp.Respository
{
    public class UserRepository : GenericRepository<Users>
    {
        // Public constructor for dependency injection
        public UserRepository() : base() { }

        /// <summary>
        /// Retrieves a user by their ID.
        /// </summary>
        /// <param name="userId">The ID of the user to retrieve.</param>
        /// <returns>A Task representing the asynchronous operation. The task result contains the Users object if found, or null if not found.</returns>
        public new async Task<Users> GetByIdAsync(int userId)
        {
            await InitializeAsync();
            try
            {
                return await (await _database.Table<Users>()).FirstOrDefaultAsync(u => u.UserID == userId);
            }
            catch (Exception ex)
            {
                LogError(nameof(GetByIdAsync), ex);
                return null;
            }
        }

        /// <summary>
        /// Retrieves all users from the database.
        /// </summary>
        /// <returns>A Task representing the asynchronous operation. The task result contains a List of Users objects.</returns>
        public new async Task<List<Users>> GetAllAsync()
        {
            await InitializeAsync();
            try
            {
                return await (await _database.Table<Users>()).ToListAsync();
            }
            catch (Exception ex)
            {
                LogError(nameof(GetAllAsync), ex);
                return new List<Users>();
            }
        }

        /// <summary>
        /// Retrieves a user by their username.
        /// </summary>
        /// <param name="username">The username of the user to retrieve.</param>
        /// <returns>A Task representing the asynchronous operation. The task result contains the Users object if found, or null if not found.</returns>
        public async Task<Users> GetUserByUsernameAsync(string username)
        {
            await InitializeAsync();
            try
            {
                return await (await _database.Table<Users>()).FirstOrDefaultAsync(u => u.Username == username);
            }
            catch (Exception ex)
            {
                LogError(nameof(GetUserByUsernameAsync), ex);
                return null;
            }
        }

        private void LogError(string methodName, Exception ex)
        {
            Console.WriteLine($"Error in {methodName}: {ex.Message}");
        }
    }
}
