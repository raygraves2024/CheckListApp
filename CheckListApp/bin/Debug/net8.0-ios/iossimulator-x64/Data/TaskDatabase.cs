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
            return await _database.InsertAsync(entity);
        }

        public async Task<int> UpdateAsync<T>(T entity) where T : new()
        {
            await EnsureDatabaseInitializedAsync();
            return await _database.UpdateAsync(entity);
        }

        public async Task<int> DeleteAsync<T>(T entity) where T : new()
        {
            await EnsureDatabaseInitializedAsync();
            return await _database.DeleteAsync(entity);
        }

        public async Task<List<UserTask>> GetTasksForUserAsync(int userId, int limit = 100, int offset = 0)
        {
            await EnsureDatabaseInitializedAsync();
            return await _database.Table<UserTask>()
                                  .Where(t => t.UserId == userId)
                                  .Skip(offset)
                                  .Take(limit)
                                  .ToListAsync();
        }

        public async Task<UserTask> GetTaskAsync(int userId, int taskId)
        {
            await EnsureDatabaseInitializedAsync();
            return await _database.Table<UserTask>()
                                  .Where(t => t.UserId == userId && t.TaskID == taskId)
                                  .FirstOrDefaultAsync();
        }
        public async Task ExecuteAsync(string sql)
        {
            await EnsureDatabaseInitializedAsync();
            await _database.ExecuteAsync(sql);
        }
    }
}