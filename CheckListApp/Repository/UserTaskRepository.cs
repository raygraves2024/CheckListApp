using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using CheckListApp.Model;
using SQLite;
using CheckListApp.Data;

namespace CheckListApp.Respository
{
    public class UserTaskRepository : GenericRepository<UserTask>
    {
        private static UserTaskRepository _instance;
        private static readonly object _lock = new object();
        private readonly TaskDatabase _database;
        private int _currentUserId;

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
        public void SetCurrentUser(int userId)
        {
            _currentUserId = userId;
        }
        public async Task<bool> CreateTaskAsync(string title, string description, int priorityLevel, string category, DateTime dueDate, bool isRepeating, string repeatInterval)
        {
            if (_currentUserId <= 0)
            {
                throw new InvalidOperationException("Current user must be set before creating a task.");
            }

            var newTask = new UserTask
            {
                UserId = _currentUserId,
                Title = title,
                Description = description,
                PriorityLevel = priorityLevel,
                Category = category,
                DueDate = dueDate,
                IsRepeating = isRepeating,
                RepeatInterval = repeatInterval,
                CreatedDate = DateTime.UtcNow,
                UpdatedDate = DateTime.UtcNow
            };

            try
            {
                var result = await _database.InsertAsync(newTask);
                return result > 0; // If result is greater than 0, it means task was successfully created
            }
            catch (Exception ex)
            {
                // Handle logging or other error management here
                return false;
            }
        }
        public async Task<List<UserTask>> GetTasksForCurrentUserAsync()
        {
            if (_currentUserId <= 0)
            {
                throw new InvalidOperationException("Current user must be set before retrieving tasks.");
            }

            return await _database.GetTasksForUserAsync(_currentUserId);
        }
        public async Task<UserTask> GetTaskForCurrentUserAsync(int taskId)
        {
            if (_currentUserId <= 0)
            {
                throw new InvalidOperationException("Current user must be set before retrieving a task.");
            }

            return await _database.GetTaskAsync(_currentUserId, taskId);
        }
        public async Task<bool> UpdateTaskAsync(int taskId, string title, string description, int priorityLevel, string category, DateTime dueDate, bool isRepeating, string repeatInterval, bool isCompleted)
        {
            if (_currentUserId <= 0)
            {
                throw new InvalidOperationException("Current user must be set before updating a task.");
            }

            var taskToUpdate = await GetTaskForCurrentUserAsync(taskId);
            if (taskToUpdate == null || taskToUpdate.UserId != _currentUserId)
            {
                return false; // Task not found or does not belong to the current user
            }

            taskToUpdate.Title = title;
            taskToUpdate.Description = description;
            taskToUpdate.PriorityLevel = priorityLevel;
            taskToUpdate.Category = category;
            taskToUpdate.DueDate = dueDate;
            taskToUpdate.IsRepeating = isRepeating;
            taskToUpdate.RepeatInterval = repeatInterval;
            taskToUpdate.IsCompleted = isCompleted;
            taskToUpdate.UpdatedDate = DateTime.UtcNow;

            try
            {
                var result = await _database.UpdateAsync(taskToUpdate);
                return result > 0; // If result is greater than 0, it means task was successfully updated
            }
            catch (Exception ex)
            {
                // Handle logging or other error management here
                return false;
            }
        }
        public async Task<bool> DeleteTaskAsync(int taskId)
        {
            if (_currentUserId <= 0)
            {
                throw new InvalidOperationException("Current user must be set before deleting a task.");
            }

            var taskToDelete = await GetTaskForCurrentUserAsync(taskId);
            if (taskToDelete == null || taskToDelete.UserId != _currentUserId)
            {
                return false; // Task not found or does not belong to the current user
            }

            try
            {
                var result = await _database.DeleteAsync(taskToDelete);
                return result > 0; // If result is greater than 0, it means task was successfully deleted
            }
            catch (Exception ex)
            {
                // Handle logging or other error management here
                return false;
            }
        }
    }
}