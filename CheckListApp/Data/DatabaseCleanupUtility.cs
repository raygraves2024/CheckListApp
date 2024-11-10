using Microsoft.Extensions.DependencyInjection;
using System.Diagnostics;
using SQLite;

namespace CheckListApp.Data
{
    public class DatabaseCleanupOptions
    {
        public bool ClearUsers { get; set; } = true;

        public bool ClearTasks { get; set; } = true;
        public bool ClearComments { get; set; } = false;
        public bool ClearNotifications { get; set; } = false;
        public bool ResetSequences { get; set; } = false;
    }

    public static class DatabaseCleanupUtility
    {

        public static async Task ClearData(IServiceProvider serviceProvider, DatabaseCleanupOptions options)
        {
            Debug.WriteLine($"Clear users value before try: {options.ClearUsers}");
            try
            {
                var database = serviceProvider.GetRequiredService<TaskDatabase>();
                await database.InitializeDatabaseAsync();

                var queries = new List<string>();

                Debug.WriteLine($"Clear users value: {options.ClearUsers}");

                if (options.ClearUsers)
                    queries.Add("DELETE FROM Users");

                if (options.ClearTasks)
                    queries.Add("DELETE FROM UserTask");

                if (options.ClearComments)
                    queries.Add("DELETE FROM Comment");

                if (options.ClearNotifications)
                    queries.Add("DELETE FROM Notification");

                foreach (var query in queries)
                {
                    await database.ExecuteAsync(query);
                    Debug.WriteLine($"Executed cleanup query: {query}");
                }

                if (options.ResetSequences && queries.Any())
                {
                    var tables = new List<string>();
                    if (options.ClearUsers) tables.Add("'Users'");
                    if (options.ClearTasks) tables.Add("'UserTask'");
                    if (options.ClearComments) tables.Add("'Comment'");
                    if (options.ClearNotifications) tables.Add("'Notification'");

                    if (tables.Any())
                    {
                        var resetSequenceQuery = $"DELETE FROM sqlite_sequence WHERE name IN ({string.Join(",", tables)})";
                        await database.ExecuteAsync(resetSequenceQuery);
                        Debug.WriteLine("Reset sequences for cleared tables");
                    }
                }

                Debug.WriteLine("Database cleanup completed successfully");
            }
            catch (Exception ex)
            {
                Debug.WriteLine($"Error during database cleanup: {ex.Message}");
                throw;
            }
        }

        // Convenience method to clear all data (maintains backward compatibility)
        public static Task ClearAllData(IServiceProvider serviceProvider)
        {
            var options = new DatabaseCleanupOptions
            {
                ClearUsers = true,
                ClearTasks = true,
                ClearComments = true,
                ClearNotifications = true,
                ResetSequences = true
            };

            return ClearData(serviceProvider, options);
        }
    }
}