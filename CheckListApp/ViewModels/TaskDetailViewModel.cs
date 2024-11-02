using CheckListApp.Model;
using CheckListApp.Services;
using CheckListApp.View;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.Windows.Input;

namespace CheckListApp.ViewModels
{
    public class TaskDetailViewModel : BindableObject
    {
        private readonly UserTaskService _userTaskService;
        private readonly int _userId = 1; // Will be replaced with actual user authentication

        private ObservableCollection<UserTask> _tasks;
        public ObservableCollection<UserTask> Tasks
        {
            get => _tasks;
            set
            {
                _tasks = value;
                OnPropertyChanged(nameof(Tasks));
            }
        }

        private bool _isLoading;
        public bool IsLoading
        {
            get => _isLoading;
            set
            {
                _isLoading = value;
                OnPropertyChanged(nameof(IsLoading));
            }
        }

        public ICommand LoadTasksCommand { get; }
        public ICommand SelectTaskCommand { get; }
        public ICommand RefreshCommand { get; }

        public TaskDetailViewModel(UserTaskService userTaskService)
        {
            _userTaskService = userTaskService;
            Tasks = new ObservableCollection<UserTask>();

            LoadTasksCommand = new Command(async () => await LoadTasksAsync());
            SelectTaskCommand = new Command<UserTask>(async (task) => await SelectTaskAsync(task));
            RefreshCommand = new Command(async () => await RefreshTasksAsync());
        }

        private async Task LoadTasksAsync()
        {
            if (IsLoading) return;

            try
            {
                IsLoading = true;
                var tasks = await _userTaskService.GetTasksAsync(_userId);
                Tasks.Clear();
                foreach (var task in tasks)
                {
                    Tasks.Add(task);
                }
            }
            catch (Exception ex)
            {
                Debug.WriteLine($"Error loading tasks: {ex.Message}");
                await Shell.Current.DisplayAlert("Error", "Unable to load tasks.", "OK");
            }
            finally
            {
                IsLoading = false;
            }
        }

        private async Task RefreshTasksAsync()
        {
            await LoadTasksAsync();
        }

        private async Task SelectTaskAsync(UserTask task)
        {
            if (task == null) return;

            try
            {
                var navigationParameter = new Dictionary<string, object>
                {
                    { "id", task.TaskID }
                };
                await Shell.Current.GoToAsync($"{nameof(ItemDetailPage)}", navigationParameter);
            }
            catch (Exception ex)
            {
                Debug.WriteLine($"Navigation error: {ex.Message}");
                await Shell.Current.DisplayAlert("Error", "Unable to open task details.", "OK");
            }
        }

        // Commented out due date checking for now
        /*private async Task CheckDueDatesAsync()
        {
            try
            {
                var tasks = await _userTaskService.GetTasksForUserAsync(_userId);
                foreach (var task in tasks)
                {
                    if ((task.DueDate - DateTime.Now).TotalDays <= 1 && !task.IsCompleted)
                    {
                        await Shell.Current.DisplayAlert("Reminder", $"Task '{task.Title}' is due soon!", "OK");
                    }
                }
            }
            catch (Exception ex)
            {
                Debug.WriteLine($"Error checking due dates: {ex.Message}");
            }
        }*/
    }
}