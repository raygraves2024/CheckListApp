using CheckListApp.Services;
using CheckListApp.Model;
using CommunityToolkit.Mvvm.Input;
using System;
using System.Collections.ObjectModel; // Import for ObservableCollection
using System.Linq; // For FirstOrDefault
using System.Windows.Input;
using System.Threading.Tasks;

namespace CheckListApp.ViewModels
{
    public class TaskEntryViewModel : BindableObject
    {
        private readonly AuthenticationService _authService;
        private readonly UserTaskService _taskService;

        private string _username;
        private string _taskName;
        private string _taskDescription;
        private UserTask _selectedTask;

        public string Username
        {
            get => _username;
            set
            {
                _username = value;
                OnPropertyChanged();
            }
        }

        public string TaskName
        {
            get => _taskName;
            set
            {
                _taskName = value;
                OnPropertyChanged();
            }
        }

        public string TaskDescription
        {
            get => _taskDescription;
            set
            {
                _taskDescription = value;
                OnPropertyChanged();
            }
        }

        public ObservableCollection<UserTask> Tasks { get; } = new ObservableCollection<UserTask>();

        public UserTask SelectedTask
        {
            get => _selectedTask;
            set
            {
                _selectedTask = value;
                OnPropertyChanged();
                // You might want to clear the task name and description when selecting a task
                TaskName = _selectedTask?.Title;
                TaskDescription = _selectedTask?.Description;
            }
        }

        public ICommand SaveTaskCommand { get; }
        public ICommand DeleteTaskCommand { get; }

        public TaskEntryViewModel(AuthenticationService authService, UserTaskService taskService)
        {
            _authService = authService;
            _taskService = taskService;
            Username = _authService.CurrentUser;

            SaveTaskCommand = new AsyncRelayCommand(OnSaveTask);
            DeleteTaskCommand = new AsyncRelayCommand(OnDeleteTask);
        }

        private async Task OnSaveTask()
        {
            if (string.IsNullOrWhiteSpace(TaskName))
            {
                await Application.Current.MainPage.DisplayAlert("Error", "Task name is required.", "OK");
                return;
            }

            try
            {
                // Save the task using the service
                var userTask = new UserTask
                {
                    // UserId = Assign appropriate UserId,
                    Title = TaskName,
                    Description = TaskDescription,
                    CreatedDate = DateTime.Now,
                    UpdatedDate = DateTime.Now,
                    IsCompleted = false
                };

                if (SelectedTask == null)
                {
                    // New task
                    await _taskService.SaveTaskAsync(userTask);
                    Tasks.Add(userTask); // Add to the collection
                }
                else
                {
                    // Update existing task
                    SelectedTask.Title = TaskName;
                    SelectedTask.Description = TaskDescription;
                    SelectedTask.UpdatedDate = DateTime.Now;

                }

                await Application.Current.MainPage.DisplayAlert("Success", "Task saved successfully!", "OK");
            }
            catch (Exception ex)
            {
                // Handle any errors that might occur during the save operation
                await Application.Current.MainPage.DisplayAlert("Error", "Failed to save task: " + ex.Message, "OK");
            }
            finally
            {
                // Clear the form
                TaskName = string.Empty;
                TaskDescription = string.Empty;
                SelectedTask = null; // Clear selection after saving
            }
        }

        private async Task OnDeleteTask()
        {
            if (SelectedTask == null)
            {
                await Application.Current.MainPage.DisplayAlert("Error", "No task selected.", "OK");
                return;
            }

            try
            {
                // Delete the task using the service
                Tasks.Remove(SelectedTask); // Remove from the collection
                SelectedTask = null; // Clear selection after deletion

                await Application.Current.MainPage.DisplayAlert("Success", "Task deleted successfully!", "OK");
            }
            catch (Exception ex)
            {
                // Handle any errors that might occur during the delete operation
                await Application.Current.MainPage.DisplayAlert("Error", "Failed to delete task: " + ex.Message, "OK");
            }

        }
    }
}
