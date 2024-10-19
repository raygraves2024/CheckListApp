using System;
using System.Diagnostics;
using Microsoft.Maui.Controls;
using CheckListApp.Services;
using Microsoft.VisualBasic;
using System.Formats.Tar;
using Microsoft.EntityFrameworkCore.Update;

namespace CheckListApp.View
{
    [QueryProperty(nameof(TaskId), "id")]
    [QueryProperty(nameof(UserId), "userId")]
    public partial class ItemDetailPage : ContentPage
    {
        private readonly UserTaskService _userTaskService;

        const string TaskKey = "savedTask";
        const string DescriptionKey = "savedDecription";
        const string PriorityKey = "savedPriority";
        const string DueDateKey = "savedDueDate";
        public int TaskId { get; set; }
        public int UserId { get; set; }

        public ItemDetailPage()
        {
            InitializeComponent();
            LoadSavedData();
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
                    TaskEntry.Text = task.CreatedTask;
                    DescriptionEntry.Text = task.Description;
                    PriorityEntry.Text = $"Priority: {task.PriorityLevel}";
                    DueDateEntry.Text = $"Due Date: {task.DueDate.ToShortDateString()}";
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

        private void LoadSavedData()
        {
            // Load saved data when the page is initialized
            TaskEntry.Text = Preferences.Get(TaskKey, string.Empty);
            DescriptionEntry.Text = Preferences.Get(DescriptionKey, string.Empty);
            PriorityEntry.Text = Preferences.Get(PriorityKey, string.Empty);
            DueDateEntry.Text = Preferences.Get(DueDateKey, string.Empty);
        }

        private void OnSaveDataClicked(object sender, EventArgs e)
        {
            // Get the text from the Entry fields
            string task = TaskEntry.Text;
            string description = DescriptionEntry.Text;
            string priority = PriorityEntry.Text;
            string duedate = DueDateEntry.Text;

            // Save the data using Preferences API
            Preferences.Set(TaskKey, task);
            Preferences.Set(DescriptionKey, description);
            Preferences.Set(PriorityKey, priority);
            Preferences.Set(DueDateKey, duedate);

            // Display the saved data in the ResultLabel
            ResultLabel.Text = $"Saved Data: \nTask: {task} \nDescription: {description} \nPriority: {priority} \nDueDate: {duedate}";

            // Optionally, clear the entry fields
            TaskEntry.Text = string.Empty;
            DescriptionEntry.Text = string.Empty;
            DueDateEntry.Text = string.Empty;
        }
    }
}
