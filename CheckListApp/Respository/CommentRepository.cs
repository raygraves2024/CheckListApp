using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using CheckListApp.Model;
using SQLite;

namespace CheckListApp.Repositories
{
    public class CommentRepository : GenericRepository<Comment>
    {
        private static CommentRepository _instance;
        private static readonly object _lock = new object();

        private CommentRepository() : base() { }

        public static CommentRepository Instance
        {
            get
            {
                if (_instance == null)
                {
                    lock (_lock)
                    {
                        _instance ??= new CommentRepository();
                    }
                }
                return _instance;
            }
        }

        public async Task<List<Comment>> GetCommentsForTaskAsync(int taskId)
        {
            await InitializeAsync();
            try
            {
                return await (await _database.Table<Comment>()).Where(c => c.TaskId == taskId).ToListAsync();
            }
            catch (Exception ex)
            {
                LogError(nameof(GetCommentsForTaskAsync), ex);
                return new List<Comment>();
            }
        }

        public async Task<List<Comment>> GetCommentsForUserAsync(int userId)
        {
            await InitializeAsync();
            try
            {
                return await (await _database.Table<Comment>()).Where(c => c.UserID == userId).ToListAsync();
            }
            catch (Exception ex)
            {
                LogError(nameof(GetCommentsForUserAsync), ex);
                return new List<Comment>();
            }
        }
    }
}