using System.Diagnostics;
using CheckListApp.Services;

namespace CheckListApp.View
{
    [QueryProperty(nameof(TaskId), "id")]
    public partial class ItemDetailPage : ContentPage
    {
        private readonly UserTaskService _userTaskService;
        private readonly string _defaultUsername = "TestUser";
        private readonly int _userId = 1; // Assuming a default user ID for TestUser

        private readonly string TitleKey = "savedTitle";
        private readonly string TaskKey = "savedTask";
        private readonly string DescriptionKey = "savedDescription";
        private readonly string PriorityKey = "savedPriority";
        private readonly string DueDateKey = "savedDueDate";
        public int TaskId { get; set; }

        public ItemDetailPage()
        {
            InitializeComponent();
            _userTaskService = new UserTaskService();
            Title = $"Task Detail - {_defaultUsername}";

            // Set initial picker selection
            PriorityPicker.SelectedIndex = 0;
            // Set initial date
            DueDatePicker.Date = DateTime.Today;

            LoadSavedData();
        }

        protected override void OnAppearing()
        {
            base.OnAppearing();
           
            Debug.WriteLine($"Navigated with TaskID: {TaskId} for user: {_defaultUsername}");
            LoadTask(TaskId);
        }

        private async void LoadTask(int taskId)
        {
            Debug.WriteLine($"Attempting to load task with TaskID: {taskId} for user: {_defaultUsername}");

            try
            {
                var task = await _userTaskService.GetTaskAsync(_userId, taskId);

                if (task != null)
                {
                    TitleEntry.Text = task.Title;
                    TaskEntry.Text = task.CreatedTask;
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
                    // Set default values for new task
                    TitleEntry.Text = string.Empty;
                    TaskEntry.Text = string.Empty;
                    DescriptionEntry.Text = string.Empty;
                    PriorityPicker.SelectedIndex = 0; // Default to "Low"
                    DueDatePicker.Date = DateTime.Today;

                    Debug.WriteLine($"No task found for TaskID: {taskId}");
                }

                TaskDetailContent.IsVisible = true;
            }
            catch (Exception ex)
            {
                Debug.WriteLine($"Error loading task details: {ex.Message}");
                await DisplayAlert("Error", "Failed to load task details.", "OK");
            }
        }

        private void LoadSavedData()
        {
            if (Preferences.ContainsKey(TitleKey))
                TitleEntry.Text = Preferences.Get(TitleKey, string.Empty);

            if (Preferences.ContainsKey(TaskKey))
                TaskEntry.Text = Preferences.Get(TaskKey, string.Empty);

            if (Preferences.ContainsKey(DescriptionKey))
                DescriptionEntry.Text = Preferences.Get(DescriptionKey, string.Empty);

            if (Preferences.ContainsKey(PriorityKey))
                PriorityPicker.SelectedItem = Preferences.Get(PriorityKey, "Low");

            if (Preferences.ContainsKey(DueDateKey))
            {
                if (DateTime.TryParse(Preferences.Get(DueDateKey, DateTime.Today.ToString()), out DateTime savedDate))
                    DueDatePicker.Date = savedDate;
            }
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
                    CreatedTask = TaskEntry.Text,
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
                Preferences.Set(TitleKey, task.Title);
                Preferences.Set(TaskKey, task.CreatedTask);
                Preferences.Set(DescriptionKey, task.Description);
                Preferences.Set(PriorityKey, PriorityPicker.SelectedItem?.ToString() ?? "Low");
                Preferences.Set(DueDateKey, task.DueDate.ToString());

                await DisplayAlert("Success", "Task saved successfully!", "OK");

                // Navigate to TaskDetailList
                await Shell.Current.GoToAsync("/TaskDetailList");
            }
            catch (Exception ex)
            {
                Debug.WriteLine($"Error saving task: {ex.Message}");
                await DisplayAlert("Error", "Failed to save task.", "OK");
            }
        }
        private async void OnViewTasksClicked(object sender, EventArgs e)
        {
            await Shell.Current.GoToAsync("/TaskDetailList");
        }
    }
}
   