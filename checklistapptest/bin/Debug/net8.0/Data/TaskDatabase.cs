using System;
using System.IO;
using SQLite;
using System.Threading.Tasks;
using CheckListApp.Model;
using System.Collections.Generic;
using System.Diagnostics;
using Microsoft.Maui.Storage;

namespace CheckListApp.Data
{
    public class TaskDatabase
    {
        private static TaskDatabase _instance;
        private static readonly object _lock = new object();
        private SQLiteAsyncConnection _database;
        private const string DatabaseFilename = "checklist.db3";

        private bool _isInitialized = false;
        private readonly SemaphoreSlim _initializationLock = new SemaphoreSlim(1, 1);

        public TaskDatabase()
        {
            if (_instance == null)
            {
                lock (_lock)
                {
                    _instance ??= this;
                }
            }
        }

        public static TaskDatabase Instance => _instance ?? (_instance = new TaskDatabase());

        public async Task InitializeDatabaseAsync()
        {
            await EnsureDatabaseInitializedAsync();
        }

        private async Task EnsureDatabaseInitializedAsync()
        {
            if (_isInitialized)
                return;

            await _initializationLock.WaitAsync();
            try
            {
                if (!_isInitialized)
                {
                    string dbPath = GetDatabasePath(DatabaseFilename);
                    _database = new SQLiteAsyncConnection(dbPath, SQLiteOpenFlags.ReadWrite | SQLiteOpenFlags.Create | SQLiteOpenFlags.SharedCache);

                    await _database.CreateTableAsync<Users>();
                    await _database.CreateTableAsync<UserTask>();
                    await _database.CreateTableAsync<Comment>();
                    await _database.CreateTableAsync<Notification>();

                    _isInitialized = true;
                    Debug.WriteLine("Database initialized successfully.");
                }
            }
            catch (Exception ex)
            {
                Debug.WriteLine($"Error initializing database: {ex.Message}");
                throw;
            }
            finally
            {
                _initializationLock.Release();
            }
        }

        private static string GetDatabasePath(string dbName)
        {
            return Path.Combine(FileSystem.AppDataDirectory, dbName);
        }

        public async Task<AsyncTableQuery<T>> Table<T>() where T : new()
        {
            await EnsureDatabaseInitializedAsync();
            return _database.Table<T>();
        }

        public async Task<T> GetByIdAsync<T>(int id) where T : new()
        {
            await EnsureDatabaseInitializedAsync();
            return await _database.FindAsync<T>(id);
        }

        public async Task<List<T>> GetAllAsync<T>() where T : new()
        {
            await EnsureDatabaseInitializedAsync();
            return await _database.Table<T>().ToListAsync();
        }

        public async Task<int> InsertAsync<T>(T entity) where T : new()
        {
            await EnsureDatabaseInitializedAsync();
            try
            {
                var result = await _database.InsertAsync(entity);
                if (entity is UserTask task)
                {
                    Debug.WriteLine($"Database Insert - Task {task.TaskID}: IsCompleted = {task.IsCompleted}");
                }
                return result;
            }
            catch (Exception ex)
            {
                Debug.WriteLine($"Error in Database InsertAsync: {ex.Message}");
                throw;
            }
        }

        public async Task<int> UpdateAsync<T>(T entity) where T : new()
        {
            await EnsureDatabaseInitializedAsync();
            try
            {
                var result = await _database.UpdateAsync(entity);
                if (entity is UserTask task)
                {
                    Debug.WriteLine($"Database Update - Task {task.TaskID}: IsCompleted = {task.IsCompleted}");

                    // Verify the update with a direct query
                    var updatedTask = await _database.Table<UserTask>()
                        .Where(t => t.TaskID == task.TaskID)
                        .FirstOrDefaultAsync();
                    Debug.WriteLine($"Verification Query - Task {updatedTask.TaskID}: IsCompleted = {updatedTask.IsCompleted}");
                }
                return result;
            }
            catch (Exception ex)
            {
                Debug.WriteLine($"Error in Database UpdateAsync: {ex.Message}");
                throw;
            }
        }

        public async Task<int> DeleteAsync<T>(T entity) where T : new()
        {
            await EnsureDatabaseInitializedAsync();
            try
            {
                var result = await _database.DeleteAsync(entity);
                if (entity is UserTask task)
                {
                    Debug.WriteLine($"Database Delete - Task {task.TaskID}");
                }
                return result;
            }
            catch (Exception ex)
            {
                Debug.WriteLine($"Error in Database DeleteAsync: {ex.Message}");
                throw;
            }
        }

        public async Task<List<UserTask>> GetTasksForUserAsync(int userId, int limit = 100, int offset = 0)
        {
            await EnsureDatabaseInitializedAsync();
            try
            {
                var tasks = await _database.Table<UserTask>()
                    .Where(t => t.UserId == userId)
                    .Skip(offset)
                    .Take(limit)
                    .ToListAsync();

                Debug.WriteLine($"Retrieved {tasks.Count} tasks for user {userId}");
                foreach (var task in tasks)
                {
                    Debug.WriteLine($"Task {task.TaskID}: IsCompleted = {task.IsCompleted}");
                }

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
                var task = await _database.Table<UserTask>()
                    .Where(t => t.UserId == userId && t.TaskID == taskId)
                    .FirstOrDefaultAsync();

                if (task != null)
                {
                    Debug.WriteLine($"Retrieved Task {task.TaskID}: IsCompleted = {task.IsCompleted}");
                }
                else
                {
                    Debug.WriteLine($"No task found with ID {taskId} for user {userId}");
                }

                return task;
            }
            catch (Exception ex)
            {
                Debug.WriteLine($"Error in GetTaskAsync: {ex.Message}");
                throw;
            }
        }

        public async Task ExecuteAsync(string sql)
        {
            await EnsureDatabaseInitializedAsync();
            try
            {
                await _database.ExecuteAsync(sql);
                Debug.WriteLine($"Executed SQL: {sql}");
            }
            catch (Exception ex)
            {
                Debug.WriteLine($"Error executing SQL: {ex.Message}");
                throw;
            }
        }

        public async Task<List<T>> QueryAsync<T>(string query) where T : new()
        {
            await EnsureDatabaseInitializedAsync();
            try
            {
                var results = await _database.QueryAsync<T>(query);
                Debug.WriteLine($"Query executed: {query}");
                Debug.WriteLine($"Results count: {results.Count}");
                return results;
            }
            catch (Exception ex)
            {
                Debug.WriteLine($"Error in QueryAsync: {ex.Message}");
                throw;
            }
        }
    }
}