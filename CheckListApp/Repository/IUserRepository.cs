using System.Collections.Generic;
using System.Threading.Tasks;
using CheckListApp.Model;

namespace CheckListApp.Repository
{
    public interface IUserRepository
    {
        /// <summary>
        /// Retrieves a user by their ID.
        /// </summary>
        Task<Users> GetByIdAsync(int userId);

        /// <summary>
        /// Retrieves all users from the database.
        /// </summary>
        Task<List<Users>> GetAllAsync();

        /// <summary>
        /// Retrieves a user by their username.
        /// </summary>
        Task<Users> GetUserByUsernameAsync(string username);

        /// <summary>
        /// Retrieves a user by their email address.
        /// </summary>
        Task<Users> GetUserByEmailAsync(string email);

        /// <summary>
        /// Adds a new user to the database.
        /// </summary>
        Task<int> AddAsync(Users user);

        /// <summary>
        /// Updates an existing user in the database.
        /// </summary>
        Task<int> UpdateAsync(Users user);
    }
}