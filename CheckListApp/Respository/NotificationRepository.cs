using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using CheckListApp.Model;
using SQLite;

namespace CheckListApp.Repositories
{
    public class NotificationRepository : GenericRepository<Notification>
    {
        private static NotificationRepository _instance;
        private static readonly object _lock = new object();

        private NotificationRepository() : base() { }

        public static NotificationRepository Instance
        {
            get
            {
                if (_instance == null)
                {
                    lock (_lock)
                    {
                        _instance ??= new NotificationRepository();
                    }
                }
                return _instance;
            }
        }

        public async Task<List<Notification>> GetNotificationsForUserAsync(int userId)
        {
            await InitializeAsync();
            try
            {
                return await (await _database.Table<Notification>()).Where(n => n.UserID == userId).ToListAsync();
            }
            catch (Exception ex)
            {
                LogError(nameof(GetNotificationsForUserAsync), ex);
                return new List<Notification>();
            }
        }

        public async Task<List<Notification>> GetUnreadNotificationsForUserAsync(int userId)
        {
            await InitializeAsync();
            try
            {
                return await (await _database.Table<Notification>()).Where(n => n.UserID == userId && !n.IsRead).ToListAsync();
            }
            catch (Exception ex)
            {
                LogError(nameof(GetUnreadNotificationsForUserAsync), ex);
                return new List<Notification>();
            }
        }

        public async Task<int> MarkNotificationAsReadAsync(int notificationId)
        {
            await InitializeAsync();
            try
            {
                var notification = await GetByIdAsync(notificationId);
                if (notification != null)
                {
                    notification.IsRead = true;
                    return await UpdateAsync(notification);
                }
                return 0;
            }
            catch (Exception ex)
            {
                LogError(nameof(MarkNotificationAsReadAsync), ex);
                return 0;
            }
        }

        public async Task<List<Notification>> GetNotificationsForTaskAsync(int taskId)
        {
            await InitializeAsync();
            try
            {
                return await (await _database.Table<Notification>()).Where(n => n.TaskID == taskId).ToListAsync();
            }
            catch (Exception ex)
            {
                LogError(nameof(GetNotificationsForTaskAsync), ex);
                return new List<Notification>();
            }
        }
    }
}