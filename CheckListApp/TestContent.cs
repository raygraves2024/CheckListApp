using System;
using System.Threading.Tasks;
using System.Collections.Generic;
using CheckListApp.Model;
using CheckListApp.Respository;

namespace CheckListApp
{
    public class TestContent
    {
        private readonly UserRepository _userRepo;
        private readonly UserTaskRepository _taskRepo;
        private readonly CommentRepository _commentRepo;
        private readonly NotificationRepository _notificationRepo;

        public TestContent(UserRepository userRepo, UserTaskRepository taskRepo, CommentRepository commentRepo, NotificationRepository notificationRepo)
        {
            _userRepo = userRepo;
            _taskRepo = taskRepo;
            _commentRepo = commentRepo;
            _notificationRepo = notificationRepo;
        }

        public async Task DumpAllTableContents()
        {
            await DumpUsers();
            await DumpUserTasks();
            await DumpComments();
            await DumpNotifications();
        }

        private async Task DumpUsers()
        {
            Console.WriteLine("Dumping Users Table:");
            var users = await _userRepo.GetAllAsync();
            foreach (var user in users)
            {
                Console.WriteLine($"User ID: {user.UserID}, Username: {user.Username}, Email: {user.Email}");
            }
            Console.WriteLine();
        }

        private async Task DumpUserTasks()
        {
            Console.WriteLine("Dumping UserTasks Table:");
            var tasks = await _taskRepo.GetAllAsync();
            foreach (var task in tasks)
            {
                Console.WriteLine($"Task ID: {task.TaskID}, User ID: {task.UserId}, Title: {task.Title}");
            }
            Console.WriteLine();
        }

        private async Task DumpComments()
        {
            Console.WriteLine("Dumping Comments Table:");
            var comments = await _commentRepo.GetAllAsync();
            foreach (var comment in comments)
            {
                Console.WriteLine($"Comment ID: {comment.CommentID}, Task ID: {comment.TaskId}, User ID: {comment.UserID}, Text: {comment.CommentText}");
            }
            Console.WriteLine();
        }

        private async Task DumpNotifications()
        {
            Console.WriteLine("Dumping Notifications Table:");
            var notifications = await _notificationRepo.GetAllAsync();
            foreach (var notification in notifications)
            {
                Console.WriteLine($"Notification ID: {notification.NotificationID}, User ID: {notification.UserID}, Message: {notification.Message}, Is Read: {notification.IsRead}");
            }
            Console.WriteLine();
        }
    }
}