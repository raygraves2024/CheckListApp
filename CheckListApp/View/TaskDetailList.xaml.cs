using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Diagnostics;
using System.Windows.Input;
using CheckListApp.Model;
using CheckListApp.Services;

namespace CheckListApp.View
{
    [QueryProperty(nameof(UserId), "userId")]
    public partial class TaskDetailList : ContentPage, INotifyPropertyChanged
    {
        private readonly UserTaskService _userTaskService;
        private ObservableCollection<UserTask> _tasks;
        private bool _isLoading;
        private int _userId;

        public int UserId
        {
            get => _userId;
            set
            {
                _userId = value;
                Debug.WriteLine($"TaskDetailList UserId set to: {_userId}");
                LoadTasks().ConfigureAwait(false);
            }
        }

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

        public ICommand RefreshCommand { get; private set; }
        public ICommand DeleteTaskCommand { get; private set; }
        public ICommand EditTaskCommand { get; private set; }
        public ICommand OpenTaskCommand { get; private set; }

        public TaskDetailList()
        {
            InitializeComponent();
            _userTaskService = new UserTaskService();
            Tasks = new ObservableCollection<UserTask>();
            InitializeCommands();
            BindingContext = this;
            Debug.WriteLine("TaskDetailList initialized");
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
                Debug.WriteLine($"Editing task for UserId: {_userId}");
                var navigationParameter = new Dictionary<string, object>
                {
                    { "userId", _userId },
                    { "task", task }
                };
                await Shell.Current.GoToAsync($"//{nameof(TaskEntryPage)}", navigationParameter);
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
                Debug.WriteLine($"Loading tasks for UserId: {_userId}");
                var tasks = await _userTaskService.GetTasksAsync(_userId);

                MainThread.BeginInvokeOnMainThread(() =>
                {
                    Tasks = tasks != null ?
                        new ObservableCollection<UserTask>(tasks) :
                        new ObservableCollection<UserTask>();
                    Debug.WriteLine($"Loaded {Tasks.Count} tasks");
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
                    Debug.WriteLine($"Deleting task {task.TaskID} for UserId: {_userId}");
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
                Debug.WriteLine($"Opening task details for TaskID: {task.TaskID} and UserId: {_userId}");
                var navigationParameter = new Dictionary<string, object>
                {
                    { "userId", _userId },
                    { "taskId", task.TaskID }
                };
                await Shell.Current.GoToAsync($"//ItemDetailPage", navigationParameter);
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
            Debug.WriteLine($"TaskDetailList appeared - Current UserId: {_userId}");
            await LoadTasks();
        }

        private async void OnBackButtonClicked(object sender, EventArgs e)
        {
            try
            {
                Debug.WriteLine("Navigating back to UserTaskPage");
                await Shell.Current.GoToAsync($"//{nameof(UserTaskPage)}");
            }
            catch (Exception ex)
            {
                Debug.WriteLine($"Error navigating back: {ex.Message}");
                await DisplayAlert("Error", "Unable to navigate back.", "OK");
            }
        }

        public new event PropertyChangedEventHandler PropertyChanged;

        protected virtual void OnPropertyChanged(string propertyName)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}