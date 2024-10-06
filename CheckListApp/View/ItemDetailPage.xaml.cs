using System;
using System.Diagnostics;
using Microsoft.Maui.Controls;
using CheckListApp.Services;

namespace CheckListApp.View
{
    [QueryProperty(nameof(TaskId), "id")]
    [QueryProperty(nameof(UserId), "userId")]
    public partial class ItemDetailPage : ContentPage
    {
        private readonly UserTaskService _userTaskService;

        public int TaskId { get; set; }
        public int UserId { get; set; }

        public ItemDetailPage()
        {
            InitializeComponent();
            _userTaskService = new UserTaskService(); // Initialize the service to load the task details
        }

        protected override void OnAppearing()
        {
            base.OnAppearing();

            // Log the passed TaskId and UserId
            Debug.WriteLine($"Navigated with TaskID: {TaskId} and UserID: {UserId}");

            // Load the task details using TaskId and UserId
            LoadTask(TaskId);
        }

        private async void LoadTask(int taskId)
        {
            Debug.WriteLine($"Attempting to load task with TaskID: {taskId} and UserID: {UserId}");

            try
            {
                // Retrieve the task using the UserTaskService
                var task = await _userTaskService.GetTaskAsync(UserId, taskId);

                if (task != null)
                {
                    Title = task.Title;
                    TitleLabel.Text = task.Title;
                    DescriptionLabel.Text = task.Description;
                    PriorityLabel.Text = $"Priority: {task.PriorityLevel}";
                    DueDateLabel.Text = $"Due Date: {task.DueDate.ToShortDateString()}";
                    IsCompletedCheckBox.IsChecked = task.IsCompleted;

                    // Make task details visible
                    TaskDetailContent.IsVisible = true;
                }
                else
                {
                    await DisplayAlert("Error", "Task not found.", "OK");
                    Debug.WriteLine($"No task found for TaskID: {taskId} and UserID: {UserId}");
                    await Shell.Current.GoToAsync("..");
                }
            }
            catch (Exception ex)
            {
                Debug.WriteLine($"Error loading task details: {ex.Message}");
                await DisplayAlert("Error", "Failed to load task details.", "OK");
            }
        }
    }
}
