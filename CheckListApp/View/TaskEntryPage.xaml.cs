using CheckListApp.Services;
using Microsoft.Maui.Controls;
using CheckListApp.Model;
using System.Diagnostics;

namespace CheckListApp.View
{
    [QueryProperty(nameof(UserId), "userId")]
    [QueryProperty(nameof(TaskToEdit), "task")]
    [QueryProperty(nameof(IsEditing), "isEditing")]
    public partial class TaskEntryPage : ContentPage
    {
        private readonly UserTaskService _userTaskService;
        private int _userId;
        private UserTask _currentTask;
        private bool _isEditing;

        public int UserId
        {
            get => _userId;
            set
            {
                _userId = value;
                Debug.WriteLine($"TaskEntryPage UserId set to: {_userId}");
            }
        }

        public UserTask TaskToEdit
        {
            get => _currentTask;
            set
            {
                if (value != null)
                {
                    _currentTask = value;
                    LoadTaskData();
                    Debug.WriteLine($"TaskEntryPage loaded existing task: {_currentTask.TaskID}");
                }
            }
        }

        public bool IsEditing
        {
            get => _isEditing;
            set
            {
                _isEditing = value;
                if (_isEditing)
                {
                    Title = "Edit Task";
                    SaveButton.Text = "Update Task";
                }
                else
                {
                    Title = "New Task";
                    SaveButton.Text = "Add Task";
                }
            }
        }

        public TaskEntryPage(UserTaskService userTaskService)
        {
            InitializeComponent();
            _userTaskService = userTaskService;
            InitializeNewTask();
        }

        private void InitializeNewTask()
        {
            _currentTask = new UserTask
            {
                DueDate = DateTime.Today,
                CreatedDate = DateTime.Now,
                UpdatedDate = DateTime.Now,
                PriorityLevel = 1,  // Default to Low
                IsCompleted = false
            };
            BindingContext = _currentTask;
            PriorityPicker.SelectedIndex = 0;
        }

        private void LoadTaskData()
        {
            if (_currentTask != null)
            {
                BindingContext = _currentTask;
                PriorityPicker.SelectedIndex = _currentTask.PriorityLevel - 1;
                TaskTitleEntry.Text = _currentTask.Title;
                TaskDescriptionEditor.Text = _currentTask.Description;
                TaskDatePicker.Date = _currentTask.DueDate;
                CompletedCheckBox.IsChecked = _currentTask.IsCompleted;
            }
        }

        private async void OnSaveClicked(object sender, EventArgs e)
        {
            try
            {
                if (string.IsNullOrWhiteSpace(TaskTitleEntry.Text))
                {
                    await DisplayAlert("Error", "Title is required", "OK");
                    return;
                }

                _currentTask.UserId = _userId;
                _currentTask.Title = TaskTitleEntry.Text;
                _currentTask.Description = TaskDescriptionEditor.Text;
                _currentTask.DueDate = TaskDatePicker.Date;
                _currentTask.IsCompleted = CompletedCheckBox.IsChecked;
                _currentTask.UpdatedDate = DateTime.Now;
                _currentTask.PriorityLevel = PriorityPicker.SelectedIndex + 1;

                if (!_isEditing)
                {
                    _currentTask.CreatedDate = DateTime.Now;
                }

                Debug.WriteLine($"Saving task for UserId: {_userId}");
                await _userTaskService.SaveTaskAsync(_currentTask);

                // Navigate to UserTaskPage
                await Shell.Current.GoToAsync($"//{nameof(UserTaskPage)}");
            }
            catch (Exception ex)
            {
                Debug.WriteLine($"Error saving task: {ex.Message}");
                await DisplayAlert("Error", "Failed to save task", "OK");
            }
        }

        private void OnPrioritySelected(object sender, EventArgs e)
        {
            if (PriorityPicker.SelectedIndex != -1)
            {
                _currentTask.PriorityLevel = PriorityPicker.SelectedIndex + 1;
                Debug.WriteLine($"Priority Level set to: {_currentTask.PriorityLevel}");
            }
        }

        protected override void OnAppearing()
        {
            base.OnAppearing();
            NavigationPage.SetHasNavigationBar(this, true);
            NavigationPage.SetHasBackButton(this, true);
        }

        private async void OnCancelClicked(object sender, EventArgs e)
        {
            try
            {
                await Shell.Current.GoToAsync($"//{nameof(UserTaskPage)}");
            }
            catch (Exception ex)
            {
                Debug.WriteLine($"Error navigating back: {ex.Message}");
                await DisplayAlert("Error", "Unable to navigate back", "OK");
            }
        }
    }
}