using System;
using System.Collections.ObjectModel;
using System.Threading.Tasks;
using System.Diagnostics;
using CheckListApp.Model;
using CheckListApp.Services;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using Microsoft.Maui.Controls;
using CheckListApp.View;
using System.Windows.Input;

namespace CheckListApp.ViewModels
{
    public partial class UserTaskViewModel : ObservableObject
    {
        private readonly UserTaskService _userTaskService;
        private readonly UserService _userService;

        [ObservableProperty]
        private ObservableCollection<UserTask> userTasks;

        [ObservableProperty]
        private UserTask selectedTask;

        [ObservableProperty]
        private Users currentUser;

        [ObservableProperty]
        private bool isLoading;

        [ObservableProperty]
        private string errorMessage;

        [ObservableProperty]
        private string userFullName;

        [ObservableProperty]
        private DateTime currentDate = DateTime.Today;

        public IAsyncRelayCommand LoadUserAndTasksCommand { get; }
        public IAsyncRelayCommand<UserTask> SelectTaskCommand { get; }
        public IAsyncRelayCommand<UserTask> DeleteTaskCommand { get; }
        public IAsyncRelayCommand<UserTask> ToggleTaskCompletionCommand { get; }
        public IAsyncRelayCommand<UserTask> EditTaskCommand { get; }

        public UserTaskViewModel(UserTaskService userTaskService, UserService userService)
        {
            _userTaskService = userTaskService ?? throw new ArgumentNullException(nameof(userTaskService));
            _userService = userService ?? throw new ArgumentNullException(nameof(userService));

            UserTasks = new ObservableCollection<UserTask>();

            LoadUserAndTasksCommand = new AsyncRelayCommand(LoadUserAndTasksAsync);
            SelectTaskCommand = new AsyncRelayCommand<UserTask>(SelectTaskAsync);
            DeleteTaskCommand = new AsyncRelayCommand<UserTask>(DeleteTaskAsync);
            ToggleTaskCompletionCommand = new AsyncRelayCommand<UserTask>(ToggleTaskCompletionAsync);
            EditTaskCommand = new AsyncRelayCommand<UserTask>(EditTaskAsync);
        }

        private async Task LoadUserAndTasksAsync()
        {
            if (IsLoading) return;

            IsLoading = true;
            ErrorMessage = string.Empty;

            try
            {
                // Fetch the current user
                CurrentUser = await _userService.GetFirstUserAsync();
                if (CurrentUser == null)
                {
                    ErrorMessage = "User not found. Please ensure you're logged in.";
                    return;
                }

                UserFullName = $"{CurrentUser.FirstName} {CurrentUser.LastName}";
                Debug.WriteLine($"Fetching tasks for user {CurrentUser.UserID}");

                // Fetch and sort tasks in a single operation
                var tasks = await _userTaskService.GetTasksForUserAsync(CurrentUser.UserID);

                // Log the initial state
                Debug.WriteLine($"Total tasks retrieved: {tasks.Count()}");
                Debug.WriteLine($"Incomplete tasks: {tasks.Count(t => !t.IsCompleted)}");
                Debug.WriteLine($"Complete tasks: {tasks.Count(t => t.IsCompleted)}");

                // Sort tasks with proper ordering
                var sortedTasks = tasks
                    .OrderBy(t => t.IsCompleted) // Incomplete first
                    .ThenByDescending(t => !t.IsCompleted ? t.PriorityLevel : 0) // Priority for incomplete
                    .ThenBy(t => !t.IsCompleted ? t.DueDate : DateTime.MaxValue) // Due date for incomplete
                    .ThenByDescending(t => t.IsCompleted ? t.UpdatedDate : DateTime.MinValue) // Update date for complete
                    .ToList();

                // Update the ObservableCollection
                UserTasks.Clear();
                foreach (var task in sortedTasks)
                {
                    UserTasks.Add(task);
                    Debug.WriteLine($"Added task - ID: {task.TaskID}, Title: {task.Title}, IsCompleted: {task.IsCompleted}");
                }
            }
            catch (Exception ex)
            {
                ErrorMessage = $"Error loading data: {ex.Message}";
                Debug.WriteLine($"Error in LoadUserAndTasksAsync: {ex}");
                await Application.Current.MainPage.DisplayAlert("Error",
                    "Failed to load tasks. Please try again.", "OK");
            }
            finally
            {
                IsLoading = false;
            }
        }

        private async Task SelectTaskAsync(UserTask task)
        {
            if (task == null) return;

            try
            {
                SelectedTask = task;
                Debug.WriteLine($"Navigating to task entry page for task: {task.TaskID}");
                var navigationParameter = new Dictionary<string, object>
                {
                    { "userId", task.UserId },
                    { "task", task },
                    { "isEditing", true }
                };
                await Shell.Current.GoToAsync($"{nameof(TaskEntryPage)}", navigationParameter);
            }
            catch (Exception ex)
            {
                Debug.WriteLine($"Navigation error: {ex.Message}");
                await Application.Current.MainPage.DisplayAlert("Error",
                    "Unable to open task details.", "OK");
            }
        }

        private async Task EditTaskAsync(UserTask task)
        {
            if (task == null) return;

            try
            {
                Debug.WriteLine($"Navigating to task entry page for editing - TaskID: {task.TaskID}");
                var navigationParameter = new Dictionary<string, object>
                {
                    { "userId", task.UserId },
                    { "task", task },
                    { "isEditing", true }
                };
                await Shell.Current.GoToAsync($"{nameof(TaskEntryPage)}", navigationParameter);
            }
            catch (Exception ex)
            {
                Debug.WriteLine($"Navigation error in EditTaskAsync: {ex.Message}");
                await Application.Current.MainPage.DisplayAlert("Error",
                    "Unable to edit task. Please try again.", "OK");
            }
        }

        private async Task DeleteTaskAsync(UserTask task)
        {
            if (task == null) return;

            try
            {
                await _userTaskService.DeleteTaskAsync(task.TaskID);
                UserTasks.Remove(task);
                Debug.WriteLine($"Task {task.TaskID} deleted successfully");
            }
            catch (Exception ex)
            {
                ErrorMessage = $"Error deleting task: {ex.Message}";
                Debug.WriteLine($"Error in DeleteTaskAsync: {ex}");
                await Application.Current.MainPage.DisplayAlert("Error",
                    "Failed to delete task. Please try again.", "OK");
            }
        }

        private async Task ToggleTaskCompletionAsync(UserTask task)
        {
            if (task == null) return;

            try
            {
                Debug.WriteLine($"Starting toggle completion for Task {task.TaskID}");
                Debug.WriteLine($"Current state - IsCompleted: {task.IsCompleted}");

                // Toggle completion state and update timestamp
                task.IsCompleted = !task.IsCompleted;
                task.UpdatedDate = DateTime.Now;

                // Update in database
                bool success = await _userTaskService.UpdateTaskAsync(task);
                if (success)
                {
                    Debug.WriteLine($"Task {task.TaskID} updated in database - IsCompleted: {task.IsCompleted}");

                    // Get current list minus the updated task
                    var currentTasks = UserTasks.Where(t => t.TaskID != task.TaskID).ToList();
                    currentTasks.Add(task);

                    // Re-sort and update the collection
                    var sortedTasks = currentTasks
                        .OrderBy(t => t.IsCompleted)
                        .ThenByDescending(t => !t.IsCompleted ? t.PriorityLevel : 0)
                        .ThenBy(t => !t.IsCompleted ? t.DueDate : DateTime.MaxValue)
                        .ThenByDescending(t => t.IsCompleted ? t.UpdatedDate : DateTime.MinValue)
                        .ToList();

                    // Update the observable collection
                    UserTasks.Clear();
                    foreach (var t in sortedTasks)
                    {
                        UserTasks.Add(t);
                    }
                }
                else
                {
                    // Revert state if update failed
                    task.IsCompleted = !task.IsCompleted;
                    Debug.WriteLine($"Failed to update task {task.TaskID} in database");
                    throw new Exception("Failed to update task in database");
                }
            }
            catch (Exception ex)
            {
                Debug.WriteLine($"Error in ToggleTaskCompletionAsync: {ex.Message}");
                await Application.Current.MainPage.DisplayAlert("Error",
                    "Failed to update task status. Please try again.", "OK");
            }
        }

        public void Dispose()
        {
            // Cleanup code if needed
            UserTasks?.Clear();
            SelectedTask = null;
            CurrentUser = null;
        }
    }
}