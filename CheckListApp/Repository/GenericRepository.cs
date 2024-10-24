using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using System.Diagnostics;
using CheckListApp.Data;
using SQLite;

namespace CheckListApp.Respository
{
    public abstract class GenericRepository<T> where T : new()
    {
        protected readonly TaskDatabase _database;

        protected GenericRepository()
        {
            _database = TaskDatabase.Instance;
        }

        protected async Task InitializeAsync()
        {
            await _database.InitializeDatabaseAsync();
        }

        public async Task<T> GetByIdAsync<TId>(TId id)
        {
            await InitializeAsync();
            try
            {
                return await _database.GetByIdAsync<T>(Convert.ToInt32(id));
            }
            catch (Exception ex)
            {
                LogError(nameof(GetByIdAsync), ex);
                return default;
            }
        }

        public async Task<List<T>> GetAllAsync()
        {
            await InitializeAsync();
            try
            {
                return await _database.GetAllAsync<T>();
            }
            catch (Exception ex)
            {
                LogError(nameof(GetAllAsync), ex);
                return new List<T>();
            }
        }

        public async Task<int> AddAsync(T entity)
        {
            await InitializeAsync();
            try
            {
                return await _database.InsertAsync(entity);
            }
            catch (Exception ex)
            {
                LogError(nameof(AddAsync), ex);
                return -1;
            }
        }

        public async Task<int> UpdateAsync(T entity)
        {
            await InitializeAsync();
            try
            {
                return await _database.UpdateAsync(entity);
            }
            catch (Exception ex)
            {
                LogError(nameof(UpdateAsync), ex);
                return 0;
            }
        }

        public async Task<int> DeleteAsync(T entity)
        {
            await InitializeAsync();
            try
            {
                return await _database.DeleteAsync(entity);
            }
            catch (Exception ex)
            {
                LogError(nameof(DeleteAsync), ex);
                return 0;
            }
        }

        protected void LogError(string methodName, Exception ex)
        {
            Debug.WriteLine($"Error in {GetType().Name}.{methodName}: {ex.Message}");
        }
    }
}