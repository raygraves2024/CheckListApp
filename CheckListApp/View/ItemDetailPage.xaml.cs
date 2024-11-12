using System.Diagnostics;
using CheckListApp.Services;
using Microsoft.Maui.Controls;

namespace CheckListApp.View
{
    [QueryProperty(nameof(TaskId), "id")]
    public partial class ItemDetailPage : ContentPage
    {
        private readonly UserTaskService _userTaskService;
        private readonly int _userId = 1; // Default user ID
        public int TaskId { get; set; }

        public ItemDetailPage()
        {
            InitializeComponent();
            _userTaskService = new UserTaskService();
            Title = "Task Detail";
            PriorityPicker.SelectedIndex = 0;
            DueDatePicker.Date = DateTime.Today;
            LoadSavedData(); // Initialize with saved data
        }

        protected override void OnAppearing()
        {
            base.OnAppearing();
            Debug.WriteLine($"Navigated with TaskID: {TaskId}");
            LoadTask(TaskId); // Load task details
            CheckDueDates(); // Check for upcoming due dates
        }

        // Method to load saved data from preferences
        private void LoadSavedData()
        {
            if (Preferences.ContainsKey("savedTitle"))
                TitleEntry.Text = Preferences.Get("savedTitle", string.Empty);

            if (Preferences.ContainsKey("savedDescription"))
                DescriptionEntry.Text = Preferences.Get("savedDescription", string.Empty);

            if (Preferences.ContainsKey("savedPriority"))
                PriorityPicker.SelectedItem = Preferences.Get("savedPriority", "Low");

            if (Preferences.ContainsKey("savedDueDate"))
            {
                if (DateTime.TryParse(Preferences.Get("savedDueDate", DateTime.Today.ToString()), out DateTime savedDate))
                    DueDatePicker.Date = savedDate;
            }
        }

        // Method to load a specific task by TaskId
        private async void LoadTask(int taskId)
        {
            Debug.WriteLine($"Attempting to load task with TaskID: {taskId}");

            try
            {
                var task = await _userTaskService.GetTaskAsync(_userId, taskId);

                if (task != null)
                {
                    TitleEntry.Text = task.Title;
                    DescriptionEntry.Text = task.Description;
                    PriorityPicker.SelectedItem = task.PriorityLevel switch
                    {
                        1 => "Low",
                        2 => "Important",
                        3 => "Urgent",
                        _ => "Low"
                    };
                    DueDatePicker.Date = task.DueDate;
                }
                else
                {
                    TitleEntry.Text = string.Empty;
                    DescriptionEntry.Text = string.Empty;
                    PriorityPicker.SelectedIndex = 0; // Default to "Low"
                    DueDatePicker.Date = DateTime.Today;

                    Debug.WriteLine($"No task found for TaskID: {taskId}");
                }
            }
            catch (Exception ex)
            {
                Debug.WriteLine($"Error loading task details: {ex.Message}");
                await DisplayAlert("Error", "Failed to load task details.", "OK");
            }
        }

        
    
        // Updated CheckDueDates method
        private async void CheckDueDates()
        {
            try
            {
                var tasks = await _userTaskService.GetTasksForUserAsync(_userId);
                foreach (var task in tasks)
                {
                    // Check if task is due within the next day and not completed
                    if ((task.DueDate - DateTime.Now).TotalDays <= 1 && !task.IsCompleted)
                    {
                        // Use SendNotification to notify about upcoming task
                        SendNotification(task.Title, task.DueDate);
                    }
                }
            }
            catch (Exception ex)
            {
                Debug.WriteLine($"Error checking due dates: {ex.Message}");
            }
        }



        private async void SendNotification(string taskTitle, DateTime dueDate)
        {
            string message = $"The task '{taskTitle}' is due on {dueDate:MMMM dd, yyyy}. Please check your tasks.";

            // Log the notification message
            Debug.WriteLine($"Notification: {message}");

            // Display notification alert within the app
            await DisplayAlert("Upcoming Task Due", message, "OK");
        }


        private async void OnSaveDataClicked(object sender, EventArgs e)
        {
            try
            {
                var task = new Model.UserTask
                {
                    TaskID = TaskId,
                    UserId = _userId,
                    Title = TitleEntry.Text,
                    Description = DescriptionEntry.Text,
                    PriorityLevel = PriorityPicker.SelectedItem?.ToString() switch
                    {
                        "Low" => 1,
                        "Important" => 2,
                        "Urgent" => 3,
                        _ => 1
                    },
                    DueDate = DueDatePicker.Date,
                    CreatedDate = DateTime.Now,
                    UpdatedDate = DateTime.Now
                };

                await _userTaskService.SaveTaskAsync(task);

                // Save to preferences
                Preferences.Set("savedTitle", task.Title);
                Preferences.Set("savedDescription", task.Description);
                Preferences.Set("savedPriority", PriorityPicker.SelectedItem?.ToString() ?? "Low");
                Preferences.Set("savedDueDate", task.DueDate.ToString());

                await DisplayAlert("Success", "Task saved successfully!", "OK");

                // Navigate to TaskDetailList
                await Shell.Current.GoToAsync("TaskDetailList");
            }
            catch (Exception ex)
            {
                Debug.WriteLine($"Error saving task: {ex.Message}");
                await DisplayAlert("Error", "Failed to save task.", "OK");
            }
        }

        private async void OnViewTasksClicked(object sender, EventArgs e)
        {
            await Shell.Current.GoToAsync("TaskDetailList");
        }
    }
}
