using CheckListApp.Services;
using Microsoft.Maui.Controls;
using System;
using CheckListApp.Model;

namespace CheckListApp.View
{
    public partial class TaskEntryPage : ContentPage
    {
        private readonly UserTaskService _userTaskService;
        private readonly int _userId = 1; // Placeholder for user ID

        // Parameterless constructor for framework instantiation
        public TaskEntryPage()
        {
            InitializeComponent();
        }

        public TaskEntryPage(UserTaskService userTaskService)
        {
            InitializeComponent();
            _userTaskService = userTaskService;
        }

        private async void OnSaveDataClicked(object sender, EventArgs e)
        {
            // Save task logic here
            var newTask = new UserTask
            {
                UserId = _userId,
                Title = TaskEntry.Text,
                Description = TaskDescriptionEditor.Text,
                DueDate = TaskDatePicker.Date,
                IsCompleted = false
            };

            await _userTaskService.SaveTaskAsync(newTask);

            // Navigate to TaskDetailList after saving
            await Navigation.PushAsync(new TaskDetailList(_userTaskService));
        }

        private async void OnDeleteButtonClicked(object sender, EventArgs e)
        {
            bool answer = await DisplayAlert("Delete Task", "Are you sure you want to delete this task?", "Yes", "No");
            if (answer)
            {
                // Add delete logic here
                await DisplayAlert("Success", "Task deleted successfully!", "OK");
                await Navigation.PushAsync(new TaskDetailList(_userTaskService));
            }
        }

        private void OnEditButtonClicked(object sender, EventArgs e)
        {
            // Enable editing of fields
            TaskEntry.IsEnabled = true;
            TaskDescriptionEditor.IsEnabled = true;
            TaskDatePicker.IsEnabled = true;

            // Toggle button states
            EditButton.IsEnabled = false;
            SaveButton.IsEnabled = true;
        }

        private async void OnSubmitButtonClicked(object sender, EventArgs e)
        {
            bool answer = await DisplayAlert("Submit Task", "Are you sure you want to submit this task?", "Yes", "No");
            if (answer)
            {
                await DisplayAlert("Success", "Task submitted successfully!", "OK");
                await Navigation.PushAsync(new TaskDetailList(_userTaskService));
            }
        }
    }
}