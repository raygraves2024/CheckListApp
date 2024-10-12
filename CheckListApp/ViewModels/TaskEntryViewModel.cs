using CheckListApp.Services;
using System.Windows.Input;

namespace CheckListApp.ViewModels
{
    public class TaskEntryViewModel : BindableObject
    {
        private readonly AuthenticationService _authService;
        private readonly UserTaskService _taskService;

        private string _username;
        private string _taskName;
        private string _taskDescription;

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

        public ICommand SaveTaskCommand { get; }

        public TaskEntryViewModel(AuthenticationService authService, UserTaskService taskService)
        {
            _authService = authService;
            _taskService = taskService;
            Username = _authService.CurrentUser;
            SaveTaskCommand = new Command(OnSaveTask);
        }

        private async void OnSaveTask()
        {
            if (string.IsNullOrWhiteSpace(TaskName))
            {
                await Application.Current.MainPage.DisplayAlert("Error", "Task name is required.", "OK");
                return;
            }

            // Here you would typically save the task to your database or service
            // For this example, we'll just show a success message
            await Application.Current.MainPage.DisplayAlert("Success", "Task saved successfully!", "OK");

            // Clear the form
            TaskName = string.Empty;
            TaskDescription = string.Empty;
        }
    }
}