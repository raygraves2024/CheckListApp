using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using CheckListApp.Model;
using CheckListApp.Data;
using System.Linq;
using SQLite;

namespace CheckListApp.Repository
{
    public class UserRepository : IUserRepository
    {
        private readonly TaskDatabase _database;

        public UserRepository(TaskDatabase database)
        {
            _database = database ?? throw new ArgumentNullException(nameof(database));
        }

        public async Task<Users> GetByIdAsync(int userId)
        {
            return await _database.GetByIdAsync<Users>(userId);
        }

        public async Task<List<Users>> GetAllAsync()
        {
            return await _database.GetAllAsync<Users>();
        }

        public async Task<Users> GetUserByUsernameAsync(string username)
        {
            var table = await _database.Table<Users>();
            return await table.Where(u => u.Username.ToLower() == username.ToLower())
                            .FirstOrDefaultAsync();
        }

        public async Task<Users> GetUserByEmailAsync(string email)
        {
            var table = await _database.Table<Users>();
            return await table.Where(u => u.Email.ToLower() == email.ToLower())
                            .FirstOrDefaultAsync();
        }

        public async Task<int> AddAsync(Users user)
        {
            return await _database.InsertAsync(user);
        }

        public async Task<int> UpdateAsync(Users user)
        {
            return await _database.UpdateAsync(user);
        }
    }
}