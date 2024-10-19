using System;
using System.Threading.Tasks;
using System.Diagnostics;
using CheckListApp.Data;
using CheckListApp.Model;
using CheckListApp.Respository;

namespace CheckListApp
{
    public class TestRepositories
    {
        private readonly UserRepository _userRepo;
        private readonly UserTaskRepository _taskRepo;
        private readonly CommentRepository _commentRepo;
        private readonly NotificationRepository _notificationRepo;

        public TestRepositories()
        {
            _userRepo = UserRepository.Instance;
            _taskRepo = UserTaskRepository.Instance;
            _commentRepo = CommentRepository.Instance;
            _notificationRepo = NotificationRepository.Instance;
        }

        public async Task RunAllTests()
        {
            await InitializeDatabase();
            await TestUserRepository();
            await TestUserTaskRepository();
            await TestCommentRepository();
            await TestNotificationRepository();
        }

        private async Task InitializeDatabase()
        {
            try
            {
                Console.WriteLine("Initializing database...");
                await TaskDatabase.Instance.InitializeDatabaseAsync();
                Console.WriteLine("Database initialized successfully.");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error initializing database: {ex.Message}");
            }
        }

        private async Task TestUserRepository()
        {
            try
            {
                Console.WriteLine("Testing UserRepository...");

                // Test Add
                var user = new Users
                {
                    Username = "testuser",
                    FirstName = "Test",
                    LastName = "User",
                    Email = "testuser@example.com",
                    CreatedDate = DateTime.UtcNow,
                    UpdatedDate = DateTime.UtcNow
                };
                int userId = await _userRepo.AddAsync(user);
                Console.WriteLine($"Added user with ID: {userId}");

                // Test Get
                var retrievedUser = await _userRepo.GetByIdAsync(userId);
                Console.WriteLine($"Retrieved user: {retrievedUser?.Username}");

                // Test Update
                if (retrievedUser != null)
                {
                    retrievedUser.FirstName = "Updated";
                    await _userRepo.UpdateAsync(retrievedUser);
                    Console.WriteLine("Updated user");
                }

                // Test Delete
                if (retrievedUser != null)
                {
                    await _userRepo.DeleteAsync(retrievedUser);
                    Console.WriteLine("Deleted user");
                }

                Console.WriteLine("UserRepository tests completed successfully.");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error in TestUserRepository: {ex.Message}");
            }
        }

        private async Task TestUserTaskRepository()
        {
            try
            {
                Console.WriteLine("Testing UserTaskRepository...");

                // First, add a user for the task
                var user = new Users
                {
                    Username = "taskuser",
                    Email = "taskuser@example.com",
                    CreatedDate = DateTime.UtcNow,
                    UpdatedDate = DateTime.UtcNow
                };
                int userId = await _userRepo.AddAsync(user);

                // Test Add
                var task = new UserTask
                {
                    UserId = userId,
                    Title = "Test Task",
                    Description = "This is a test task",
                    CreatedDate = DateTime.UtcNow,
                    UpdatedDate = DateTime.UtcNow
                };
                int taskId = await _taskRepo.AddAsync(task);
                Console.WriteLine($"Added task with ID: {taskId}");

                // Test Get
                var retrievedTask = await _taskRepo.GetByIdAsync(taskId);
                Console.WriteLine($"Retrieved task: {retrievedTask?.Title}");

                // Test Update
                if (retrievedTask != null)
                {
                    retrievedTask.Title = "Updated Task";
                    await _taskRepo.UpdateAsync(retrievedTask);
                    Console.WriteLine("Updated task");
                }

                // Test Delete
                if (retrievedTask != null)
                {
                    await _taskRepo.DeleteAsync(retrievedTask);
                    Console.WriteLine("Deleted task");
                }

                // Clean up the user
                await _userRepo.DeleteAsync(user);

                Console.WriteLine("UserTaskRepository tests completed successfully.");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error in TestUserTaskRepository: {ex.Message}");
            }
        }

        private async Task TestCommentRepository()
        {
            try
            {
                Console.WriteLine("Testing CommentRepository...");

                // First, add a user and a task for the comment
                var user = new Users
                {
                    Username = "commentuser",
                    Email = "commentuser@example.com",
                    CreatedDate = DateTime.UtcNow,
                    UpdatedDate = DateTime.UtcNow
                };
                int userId = await _userRepo.AddAsync(user);

                var task = new UserTask
                {
                    UserId = userId,
                    Title = "Comment Task",
                    CreatedDate = DateTime.UtcNow,
                    UpdatedDate = DateTime.UtcNow
                };
                int taskId = await _taskRepo.AddAsync(task);

                // Test Add
                var comment = new Comment
                {
                    TaskId = taskId,
                    UserID = userId,
                    CommentText = "This is a test comment",
                    CreatedDate = DateTime.UtcNow,
                    UpdatedDate = DateTime.UtcNow
                };
                int commentId = await _commentRepo.AddAsync(comment);
                Console.WriteLine($"Added comment with ID: {commentId}");

                // Test Get
                var retrievedComment = await _commentRepo.GetByIdAsync(commentId);
                Console.WriteLine($"Retrieved comment: {retrievedComment?.CommentText}");

                // Test Update
                if (retrievedComment != null)
                {
                    retrievedComment.CommentText = "Updated comment";
                    await _commentRepo.UpdateAsync(retrievedComment);
                    Console.WriteLine("Updated comment");
                }

                // Test Delete
                if (retrievedComment != null)
                {
                    await _commentRepo.DeleteAsync(retrievedComment);
                    Console.WriteLine("Deleted comment");
                }

                // Clean up the task and user
                await _taskRepo.DeleteAsync(task);
                await _userRepo.DeleteAsync(user);

                Console.WriteLine("CommentRepository tests completed successfully.");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error in TestCommentRepository: {ex.Message}");
            }
        }

        private async Task TestNotificationRepository()
        {
            try
            {
                Console.WriteLine("Testing NotificationRepository...");

                // First, add a user for the notification
                var user = new Users
                {
                    Username = "notificationuser",
                    Email = "notificationuser@example.com",
                    CreatedDate = DateTime.UtcNow,
                    UpdatedDate = DateTime.UtcNow
                };
                int userId = await _userRepo.AddAsync(user);

                // Test Add
                var notification = new Notification
                {
                    UserID = userId,
                    Message = "This is a test notification",
                    NotificationDate = DateTime.UtcNow,
                    IsRead = false,
                    CreatedDate = DateTime.UtcNow
                };
                int notificationId = await _notificationRepo.AddAsync(notification);
                Console.WriteLine($"Added notification with ID: {notificationId}");

                // Test Get
                var retrievedNotification = await _notificationRepo.GetByIdAsync(notificationId);
                Console.WriteLine($"Retrieved notification: {retrievedNotification?.Message}");

                // Test Update
                if (retrievedNotification != null)
                {
                    retrievedNotification.Message = "Updated notification";
                    await _notificationRepo.UpdateAsync(retrievedNotification);
                    Console.WriteLine("Updated notification");
                }

                // Test Mark as Read
                await _notificationRepo.MarkNotificationAsReadAsync(notificationId);
                Console.WriteLine("Marked notification as read");

                // Test Delete
                if (retrievedNotification != null)
                {
                    await _notificationRepo.DeleteAsync(retrievedNotification);
                    Console.WriteLine("Deleted notification");
                }

                // Clean up the user
                await _userRepo.DeleteAsync(user);

                Console.WriteLine("NotificationRepository tests completed successfully.");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error in TestNotificationRepository: {ex.Message}");
            }
        }
    }
}