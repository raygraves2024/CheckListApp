using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using System.Diagnostics;
using CheckListApp.Model;
using CheckListApp.Data;

namespace CheckListApp.Services
{
    public class UserTaskService
    {
        private readonly TaskDatabase _database;

        public UserTaskService()
        {
            _database = TaskDatabase.Instance;
        }

        private async Task EnsureDatabaseInitializedAsync()
        {
            try
            {
                await _database.InitializeDatabaseAsync();
            }
            catch (Exception ex)
            {
                Debug.WriteLine($"Database initialization failed: {ex.Message}");
                throw;
            }
        }

        public async Task<List<UserTask>> GetTasksAsync(int userId)
        {
            return await GetTasksForUserAsync(userId);
        }

        public async Task<List<UserTask>> GetTasksForUserAsync(int userId)
        {
            await EnsureDatabaseInitializedAsync();
            try
            {
                var tasks = await _database.GetTasksForUserAsync(userId);
                Debug.WriteLine($"Retrieved {tasks.Count} tasks for user {userId}");
                return tasks;
            }
            catch (Exception ex)
            {
                Debug.WriteLine($"Error in GetTasksForUserAsync: {ex.Message}");
                throw;
            }
        }

        public async Task<UserTask> GetTaskAsync(int userId, int taskId)
        {
            await EnsureDatabaseInitializedAsync();
            try
            {
                var task = await _database.GetTaskAsync(userId, taskId);
                Debug.WriteLine(task != null
                    ? $"Retrieved task: {task.Title} (ID: {task.TaskID})"
                    : $"No task found with ID {taskId}");
                return task;
            }
            catch (Exception ex)
            {
                Debug.WriteLine($"Error in GetTaskAsync: {ex.Message}");
                throw;
            }
        }

        public async Task<int> SaveTaskAsync(UserTask task)
        {
            await EnsureDatabaseInitializedAsync();
            try
            {
                int result;
                if (task.TaskID != 0)
                {
                    result = await _database.UpdateAsync(task);
                    Debug.WriteLine($"Updated task {task.TaskID}");
                }
                else
                {
                    result = await _database.InsertAsync(task);
                    Debug.WriteLine($"Inserted new task, ID: {result}");
                }
                return result;
            }
            catch (Exception ex)
            {
                Debug.WriteLine($"Error in SaveTaskAsync: {ex.Message}");
                throw;
            }
        }

        public async Task<bool> DeleteTaskAsync(int taskId)
        {
            await EnsureDatabaseInitializedAsync();
            try
            {
                // Get task by ID
                var query = await _database.Table<UserTask>();
                var task = await query.Where(t => t.TaskID == taskId).FirstOrDefaultAsync();

                if (task != null)
                {
                    var result = await _database.DeleteAsync(task);
                    Debug.WriteLine($"Deleted task {taskId}");
                    return result > 0;
                }
                Debug.WriteLine($"Task {taskId} not found for deletion");
                return false;
            }
            catch (Exception ex)
            {
                Debug.WriteLine($"Error in DeleteTaskAsync: {ex.Message}");
                throw;
            }
        }

        public async Task<bool> UpdateTaskAsync(UserTask task)
        {
            await EnsureDatabaseInitializedAsync();
            try
            {
                var result = await _database.UpdateAsync(task);
                Debug.WriteLine($"Updated task {task.TaskID}");
                return result > 0;
            }
            catch (Exception ex)
            {
                Debug.WriteLine($"Error in UpdateTaskAsync: {ex.Message}");
                throw;
            }
        }
    }
}