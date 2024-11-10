using Microsoft.Maui.Controls;
using CheckListApp.ViewModels;
using CheckListApp.Model;
using System.Diagnostics;

namespace CheckListApp.View
{
    public partial class UserTaskPage : ContentPage
    {
        private readonly UserTaskViewModel _viewModel;

        public UserTaskPage(UserTaskViewModel viewModel)
        {
            InitializeComponent();
            _viewModel = viewModel;
            BindingContext = viewModel;
            Debug.WriteLine("UserTaskPage initialized");
        }

        protected override async void OnAppearing()
        {
            base.OnAppearing();

            try
            {
                await _viewModel.LoadUserAndTasksCommand.ExecuteAsync(null);
                Debug.WriteLine("Tasks loaded successfully");
            }
            catch (Exception ex)
            {
                Debug.WriteLine($"Error loading tasks: {ex.Message}");
                await DisplayAlert("Error", "Unable to load tasks.", "OK");
            }
        }

        private async void OnAddTask_Clicked(object sender, EventArgs e)
        {
            try
            {
                var navigationParameter = new Dictionary<string, object>
                {
                    { "userId", _viewModel.CurrentUser?.UserID ?? 1 }
                };
                Debug.WriteLine("Navigating to TaskEntryPage for new task");
                await Shell.Current.GoToAsync($"{nameof(TaskEntryPage)}", navigationParameter);
            }
            catch (Exception ex)
            {
                Debug.WriteLine($"Error navigating to TaskEntryPage: {ex.Message}");
                await DisplayAlert("Error", "Unable to add new task.", "OK");
            }
        }

        private async void OnDeleteSwipeItemInvoked(object sender, EventArgs e)
        {
            if (sender is SwipeItem swipeItem && swipeItem.CommandParameter is UserTask task)
            {
                bool confirm = await DisplayAlert(
                    "Confirm Delete",
                    "Are you sure you want to delete this task?",
                    "Yes", "No");

                if (confirm)
                {
                    await _viewModel.DeleteTaskCommand.ExecuteAsync(task);
                }
            }
        }
    }
}