using CheckListApp.ViewModels;
using Microsoft.Maui.Controls;
using System.IO;
using System.Threading.Tasks;

namespace CheckListApp.View
{
    public partial class TaskEntryPage : ContentPage
    {
        private string fileName = Path.Combine(FileSystem.AppDataDirectory, "formData.txt");
        private bool isEditing = false;

        public TaskEntryPage(TaskEntryViewModel viewModel)
        {
            InitializeComponent();
            LoadData();
            BindingContext = viewModel;
        }

        public class UserData
        {
            public string Task { get; set; }
            public string Description { get; set; }
        }

        private async void OnSaveDataClicked(object sender, EventArgs e)
        {
            var userData = new UserData
            {
                Task = TaskEntry.Text,
                Description = TaskDescriptionEditor.Text
            };

            await SaveData(userData);
        }

        private async Task SaveData(UserData userData)
        {
            // Validate input fields
            if (string.IsNullOrWhiteSpace(userData.Task) || string.IsNullOrWhiteSpace(userData.Description))
            {
                await DisplayAlert("Error", "Please fill in all fields.", "OK");
                return;
            }

            string data = $"Task: {userData.Task}, Description: {userData.Description}";

            try
            {
                // Save data to the file
                await File.WriteAllTextAsync(fileName, data);
                await DisplayAlert("Success", "Your data has been saved!", "OK");

                // Update buttons' states
                isEditing = false;
                EditButton.IsEnabled = true;
                DeleteButton.IsEnabled = true;
            }
            catch (Exception ex)
            {
                await DisplayAlert("Error", $"An error occurred while saving: {ex.Message}", "OK");
            }
        }

        private void LoadData()
        {
            if (File.Exists(fileName))
            {
                string data = File.ReadAllText(fileName);
                var parts = data.Split(", ");
                if (parts.Length >= 2)
                {
                    TaskEntry.Text = parts[0].Replace("Task: ", "");
                    TaskDescriptionEditor.Text = parts[1].Replace("Description: ", "");

                    // Enable Edit and Delete buttons
                    EditButton.IsEnabled = true;
                    DeleteButton.IsEnabled = true;
                }
            }
        }

        private async void OnEditButtonClicked(object sender, EventArgs e)
        {
            if (!isEditing)
            {
                // Set the editing mode
                isEditing = true;
                await DisplayAlert("Edit Mode", "You can edit the fields now. Click Save to save changes.", "OK");
            }
            else
            {
                // If already in edit mode, save changes
                var userData = new UserData
                {
                    Task = TaskEntry.Text,
                    Description = TaskDescriptionEditor.Text
                };
                await SaveData(userData);
                isEditing = false;
            }
        }

        private async void OnDeleteButtonClicked(object sender, EventArgs e)
        {
            // Clear the input fields
            TaskEntry.Text = string.Empty;
            TaskDescriptionEditor.Text = string.Empty;

            // Delete the saved data from the file
            try
            {
                if (File.Exists(fileName))
                {
                    File.Delete(fileName);
                    await DisplayAlert("Success", "Your data has been deleted!", "OK");
                }
                else
                {
                    await DisplayAlert("Info", "No data found to delete.", "OK");
                }

                // Disable Edit and Delete buttons
                EditButton.IsEnabled = false;
                DeleteButton.IsEnabled = false;
            }
            catch (Exception ex)
            {
                await DisplayAlert("Error", $"An error occurred while deleting: {ex.Message}", "OK");
            }
        }
    }
}
