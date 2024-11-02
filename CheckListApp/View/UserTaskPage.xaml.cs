using Microsoft.Maui.Controls;
using CheckListApp.ViewModels;
using CheckListApp.Model;
using System.Diagnostics;

namespace CheckListApp.View
{
    public partial class UserTaskPage : ContentPage
    {
        private UserTaskViewModel _viewModel;

        // Default constructor for framework instantiation
        public UserTaskPage()
        {
            InitializeComponent();
            _viewModel = new UserTaskViewModel();
            BindingContext = _viewModel;
        }

        // Constructor with ViewModel injection
        public UserTaskPage(UserTaskViewModel viewModel)
        {
            InitializeComponent();
            _viewModel = viewModel;
            BindingContext = viewModel;
        }

        protected override async void OnAppearing()
        {
            base.OnAppearing();

            // Safely execute LoadUserAndTasksCommand if initialized
            if (_viewModel?.LoadUserAndTasksCommand != null)
            {
                try
                {
                    await _viewModel.LoadUserAndTasksCommand.ExecuteAsync(null);
                }
                catch (Exception ex)
                {
                    Debug.WriteLine($"Error loading tasks: {ex.Message}");
                    await DisplayAlert("Error", "Unable to load tasks.", "OK");
                }
            }
            else
            {
                Debug.WriteLine("ViewModel or LoadUserAndTasksCommand is null.");
                await DisplayAlert("Error", "System initialization failed.", "OK");
            }
        }

        private async void OnItemSelected(object sender, SelectionChangedEventArgs args)
        {
            if (args.CurrentSelection.Count > 0)
            {
                var task = args.CurrentSelection[0] as UserTask;
                if (task != null)
                {
                    if (_viewModel.SelectTaskCommand?.CanExecute(task) == true)
                    {
                        await _viewModel.SelectTaskCommand.ExecuteAsync(task);
                    }
                    else
                    {
                        Debug.WriteLine($"Navigating to ItemDetailPage with TaskID: {task.TaskID} and UserID: {task.UserId}");
                        await Shell.Current.GoToAsync($"{nameof(ItemDetailPage)}?id={task.TaskID}&userId={task.UserId}");
                    }
                }

                // Clear selection
                if (sender is CollectionView collectionView)
                {
                    collectionView.SelectedItem = null;
                }
            }
        }

        private async void OnAddTask_Clicked(object sender, EventArgs e)
        {
            await Navigation.PushAsync(new TaskEntryPage());
        }

        private async void OnLogout_Clicked(object sender, EventArgs e)
        {
            bool answer = await DisplayAlert("Logout", "Are you sure you want to logout?", "Yes", "No");
            if (answer)
            {
                // Add any logout logic here (clear credentials, etc.)
                await Shell.Current.GoToAsync("//LoginPage");
            }
        }
    }
}