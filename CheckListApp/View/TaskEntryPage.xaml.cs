using CheckListApp.Services;
using Microsoft.Maui.Controls;
using System;
using CheckListApp.Model;
using System.Diagnostics;

namespace CheckListApp.View
{
    public partial class TaskEntryPage : ContentPage
    {
        private readonly UserTaskService _userTaskService;
        private readonly int _userId = 1;
        private UserTask _currentTask;

        public TaskEntryPage()
        {
            InitializeComponent();
            _userTaskService = new UserTaskService();
            _currentTask = new UserTask
            {
                DueDate = DateTime.Today,
                CreatedDate = DateTime.Now,
                UpdatedDate = DateTime.Now,
                PriorityLevel = 1  // Default to Low
            };
            BindingContext = _currentTask;
            PriorityPicker.SelectedIndex = 0; // Set default to Low
        }

        public TaskEntryPage(UserTask existingTask)
        {
            InitializeComponent();
            _userTaskService = new UserTaskService();
            _currentTask = existingTask;
            BindingContext = _currentTask;

            // Convert PriorityLevel to picker index (subtract 1 since PriorityLevel starts at 1)
            PriorityPicker.SelectedIndex = existingTask.PriorityLevel - 1;
        }

        private void OnPrioritySelected(object sender, EventArgs e)
        {
            if (PriorityPicker.SelectedIndex != -1)
            {
                // Add 1 to match your priority scale (1-Low, 2-Important, 3-Urgent)
                _currentTask.PriorityLevel = PriorityPicker.SelectedIndex + 1;
                Debug.WriteLine($"Priority Level set to: {_currentTask.PriorityLevel} ({PriorityPicker.SelectedItem})");
            }
        }

        private async void OnUpdateTaskClicked(object sender, EventArgs e)
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

                // Set Created Date only if it's a new task
                if (_currentTask.CreatedDate == DateTime.MinValue)
                {
                    _currentTask.CreatedDate = DateTime.Now;
                }

                // Debug output
                Debug.WriteLine($"Updating Task:");
                Debug.WriteLine($"TaskID: {_currentTask.TaskID}");
                Debug.WriteLine($"Title: {_currentTask.Title}");
                Debug.WriteLine($"Description: {_currentTask.Description}");
                Debug.WriteLine($"Priority Level: {_currentTask.PriorityLevel} ({GetPriorityText(_currentTask.PriorityLevel)})");
                Debug.WriteLine($"DueDate: {_currentTask.DueDate}");
                Debug.WriteLine($"IsCompleted: {_currentTask.IsCompleted}");
                Debug.WriteLine($"Created Date: {_currentTask.CreatedDate}");
                Debug.WriteLine($"Updated Date: {_currentTask.UpdatedDate}");

                await _userTaskService.SaveTaskAsync(_currentTask);
                await Navigation.PushAsync(new TaskDetailList()); // Go back to previous page
            }
            catch (Exception ex)
            {
                Debug.WriteLine($"Error updating task: {ex.Message}");
                await DisplayAlert("Error", "Failed to update task", "OK");
            }
        }

        private string GetPriorityText(int priorityLevel)
        {
            return priorityLevel switch
            {
                1 => "Low",
                2 => "Important",
                3 => "Urgent",
                _ => "Unknown"
            };
        }

        protected override void OnAppearing()
        {
            base.OnAppearing();
            NavigationPage.SetHasBackButton(this, false);
            NavigationPage.SetHasNavigationBar(this, false);
        }
    }
}