using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using CheckListApp.Model;
using SQLite;

namespace CheckListApp.Repositories
{
    public class UserTaskRepository : GenericRepository<UserTask>
    {
        private static UserTaskRepository _instance;
        private static readonly object _lock = new object();

        private UserTaskRepository() : base() { }

        public static UserTaskRepository Instance
        {
            get
            {
                if (_instance == null)
                {
                    lock (_lock)
                    {
                        _instance ??= new UserTaskRepository();
                    }
                }
                return _instance;
            }
        }

        public async Task<List<UserTask>> GetTasksForUserAsync(int userId)
        {
            await InitializeAsync();
            try
            {
                return await _database.GetTasksForUserAsync(userId);
            }
            catch (Exception ex)
            {
                LogError(nameof(GetTasksForUserAsync), ex);
                return new List<UserTask>();
            }
        }

        public async Task<UserTask> GetTaskForUserAsync(int userId, int taskId)
        {
            await InitializeAsync();
            try
            {
                return await _database.GetTaskAsync(userId, taskId);
            }
            catch (Exception ex)
            {
                LogError(nameof(GetTaskForUserAsync), ex);
                return null;
            }
        }
    }
}