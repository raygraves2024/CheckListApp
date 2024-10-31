using System.Collections.ObjectModel;
using System.Diagnostics;
using CheckListApp.Model;
using CheckListApp.Services;
using Microsoft.Maui.Controls;

namespace CheckListApp.View
{
    public partial class TaskDetailList : ContentPage
    {
        private readonly UserTaskService _userTaskService;
        private readonly int _userId = 1; // Placeholder for user ID
        private ObservableCollection<UserTask> _tasks;

        // Parameterless constructor with InitializeComponent for framework instantiation
        public TaskDetailList()
        {
            InitializeComponent();
        }

        public TaskDetailList(UserTaskService userTaskService)
        {
            InitializeComponent();
            _userTaskService = userTaskService;
            LoadTasks();
        }

        private async void LoadTasks()
        {
            // Load tasks asynchronously to prevent UI blocking
            _tasks = new ObservableCollection<UserTask>(await _userTaskService.GetTasksAsync(_userId));
            TaskListView.ItemsSource = _tasks;
        }

        // Swipe to open task details
        private async void OnOpenTaskSwipe(object sender, EventArgs e)
        {
            var swipeItem = sender as SwipeItem;
            var task = swipeItem?.BindingContext as UserTask;

            if (task != null)
            {
                Debug.WriteLine($"Navigating to ItemDetailPage with TaskID: {task.TaskID}");
                await Shell.Current.GoToAsync($"{nameof(ItemDetailPage)}?id={task.TaskID}");
            }
        }
    }
}