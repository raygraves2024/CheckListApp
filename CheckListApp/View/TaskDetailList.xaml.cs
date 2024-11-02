using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Diagnostics;
using System.Windows.Input;
using CheckListApp.Model;
using CheckListApp.Services;

namespace CheckListApp.View
{
    public partial class TaskDetailList : ContentPage, INotifyPropertyChanged
    {
        private readonly UserTaskService _userTaskService;
        private readonly int _userId = 1;
        private ObservableCollection<UserTask> _tasks;
        private bool _isLoading;

        // Public properties for commands
        public ICommand RefreshCommand { get; private set; }
        public ICommand DeleteTaskCommand { get; private set; }
        public ICommand EditTaskCommand { get; private set; }
        public ICommand OpenTaskCommand { get; private set; }

        // Public property for Tasks with proper notification
        public ObservableCollection<UserTask> Tasks
        {
            get => _tasks;
            set
            {
                if (_tasks != value)
                {
                    _tasks = value;
                    OnPropertyChanged(nameof(Tasks));
                }
            }
        }

        // Public property for IsLoading with proper notification
        public bool IsLoading
        {
            get => _isLoading;
            set
            {
                if (_isLoading != value)
                {
                    _isLoading = value;
                    OnPropertyChanged(nameof(IsLoading));
                }
            }
        }

        public TaskDetailList()
        {
            InitializeComponent();
            _userTaskService = new UserTaskService();
            Tasks = new ObservableCollection<UserTask>();
            InitializeCommands();
            BindingContext = this;
        }

        private void InitializeCommands()
        {
            RefreshCommand = new Command(async () => await LoadTasks());
            DeleteTaskCommand = new Command<UserTask>(async (task) => await DeleteTask(task));
            EditTaskCommand = new Command<UserTask>(async (task) => await EditTask(task));
            OpenTaskCommand = new Command<UserTask>(async (task) => await OpenTask(task));
        }

        private async Task EditTask(UserTask task)
        {
            if (task == null) return;

            try
            {
                await Navigation.PushAsync(new TaskEntryPage(task));
            }
            catch (Exception ex)
            {
                Debug.WriteLine($"Error editing task: {ex.Message}");
                await DisplayAlert("Error", "Unable to edit task.", "OK");
            }
        }

        private async Task LoadTasks()
        {
            if (IsLoading) return;

            try
            {
                IsLoading = true;
                var tasks = await _userTaskService.GetTasksAsync(_userId);

                MainThread.BeginInvokeOnMainThread(() =>
                {
                    if (tasks != null)
                    {
                        Tasks = new ObservableCollection<UserTask>(tasks);
                    }
                    else
                    {
                        Tasks = new ObservableCollection<UserTask>();
                    }
                });
            }
            catch (Exception ex)
            {
                Debug.WriteLine($"Error loading tasks: {ex.Message}");
                await MainThread.InvokeOnMainThreadAsync(async () =>
                {
                    await DisplayAlert("Error", "Unable to load tasks.", "OK");
                    Tasks = new ObservableCollection<UserTask>();
                });
            }
            finally
            {
                IsLoading = false;
            }
        }

        private async Task DeleteTask(UserTask task)
        {
            if (task == null) return;

            bool confirmDelete = await DisplayAlert("Delete Task",
                "Are you sure you want to delete this task?", "Yes", "No");

            if (confirmDelete)
            {
                try
                {
                    IsLoading = true;
                    await _userTaskService.DeleteTaskAsync(task.TaskID);

                    MainThread.BeginInvokeOnMainThread(() =>
                    {
                        Tasks.Remove(task);
                    });

                    await DisplayAlert("Success", "Task deleted successfully.", "OK");
                }
                catch (Exception ex)
                {
                    Debug.WriteLine($"Error deleting task: {ex.Message}");
                    await DisplayAlert("Error", "Failed to delete task.", "OK");
                }
                finally
                {
                    IsLoading = false;
                }
            }
        }

        private async Task OpenTask(UserTask task)
        {
            if (task == null) return;

            try
            {
                Debug.WriteLine($"Opening task details for TaskID: {task.TaskID}");
                await Shell.Current.GoToAsync($"///ItemDetailPage?taskId={task.TaskID}");
            }
            catch (Exception ex)
            {
                Debug.WriteLine($"Error opening task: {ex.Message}");
                await DisplayAlert("Error", "Unable to open task details.", "OK");
            }
        }

        protected override async void OnAppearing()
        {
            base.OnAppearing();
            await LoadTasks();
        }

        private async void OnBackButtonClicked(object sender, EventArgs e)
        {
            try
            {
                await Shell.Current.GoToAsync("///ItemDetailPage");
            }
            catch (Exception ex)
            {
                Debug.WriteLine($"Error navigating back: {ex.Message}");
                await DisplayAlert("Error", "Unable to navigate back.", "OK");
            }
        }

        // Implement property changed notification
        public new event PropertyChangedEventHandler PropertyChanged;

        protected virtual void OnPropertyChanged(string propertyName)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}