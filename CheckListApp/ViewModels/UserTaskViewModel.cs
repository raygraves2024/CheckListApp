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

namespace CheckListApp.ViewModels
{
    public partial class UserTaskViewModel : ObservableObject
    {
        private readonly UserTaskService _userTaskService;
        private readonly UserService _userService;

        public UserTaskViewModel(UserTaskService userTaskService, UserService userService)
        {
            _userTaskService = userTaskService;
            _userService = userService;
            LoadUserAndTasksCommand = new AsyncRelayCommand(LoadUserAndTasksAsync);
            SelectTaskCommand = new AsyncRelayCommand<UserTask>(SelectTaskAsync);
            DeleteTaskCommand = new AsyncRelayCommand<UserTask>(DeleteTaskAsync);
            //RunTestsCommand = new RelayCommand(RunTests);  // Added command for running tests
        }

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

        public IAsyncRelayCommand LoadUserAndTasksCommand { get; }
        public IAsyncRelayCommand<UserTask> SelectTaskCommand { get; }
        public IAsyncRelayCommand<UserTask> DeleteTaskCommand { get; } // Added DeleteTaskCommand
        public RelayCommand RunTestsCommand { get; } // Added property for test command

        // Load the user and tasks asynchronously
        private async Task LoadUserAndTasksAsync()
        {
            IsLoading = true;
            ErrorMessage = string.Empty;

            try
            {
                // Fetch the first user
                CurrentUser = await _userService.GetFirstUserAsync();
                if (CurrentUser == null)
                {
                    ErrorMessage = "Test user not found. Ensure the database is properly initialized.";
                    return;
                }

                // Fetch tasks for the current user
                var tasks = await _userTaskService.GetTasksForUserAsync(CurrentUser.UserID);
                UserTasks = new ObservableCollection<UserTask>(tasks);

                Debug.WriteLine($"Loaded {UserTasks.Count} tasks for user {CurrentUser.UserID}");

                if (UserTasks.Count == 0)
                {
                    Debug.WriteLine("No tasks found. Ensure test data is inserted.");
                }
            }
            catch (Exception ex)
            {
                ErrorMessage = $"Error loading data: {ex.Message}";
                Debug.WriteLine($"Error in LoadUserAndTasksAsync: {ex}");
            }
            finally
            {
                IsLoading = false;
            }
        }

        // Select the task and navigate to the detail page
        private async Task SelectTaskAsync(UserTask task)
        {
            if (task != null)
            {
                SelectedTask = task;
                Debug.WriteLine($"Navigating to task detail page with TaskID: {task.TaskID} and UserID: {task.UserId}");

                // Pass the TaskID and UserID as parameters to the detail page
                await Shell.Current.GoToAsync($"{nameof(ItemDetailPage)}?id={task.TaskID}&userId={task.UserId}");
            }
        }

        // Delete the selected task asynchronously
        private async Task DeleteTaskAsync(UserTask task)
        {
            if (task == null) return;

            try
            {
                await _userTaskService.DeleteTaskAsync(task.TaskID);
                UserTasks.Remove(task);
                Debug.WriteLine($"Task with ID {task.TaskID} deleted successfully.");
            }
            catch (Exception ex)
            {
                ErrorMessage = $"Error deleting task: {ex.Message}";
                Debug.WriteLine($"Error in DeleteTaskAsync: {ex}");
            }
        }
        public UserTaskViewModel()
        {
            // Initialize any required default values if needed
            UserTasks = new ObservableCollection<UserTask>();
            IsLoading = false;
            ErrorMessage = string.Empty;
        }


        // Method to run tests
        //private void RunTests()
        //{
        //    var testRepository = new TestRepositories(); 
        //    testRepository.RunAllTests();  // Call the test method from TestRepositories
        //    Debug.WriteLine("Test repositories executed successfully.");
        //}
    }
}
