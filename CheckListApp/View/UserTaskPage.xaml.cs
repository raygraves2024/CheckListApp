using System;
using System.Diagnostics;
using CheckListApp.ViewModels;
using CheckListApp.Model;
using Microsoft.Maui.Controls;

namespace CheckListApp.View
{
    public partial class UserTaskPage : ContentPage
    {
        private UserTaskViewModel _viewModel;

        // Parameterless constructor for framework instantiation
        public UserTaskPage()
        {
            InitializeComponent();
        }

        public UserTaskPage(UserTaskViewModel viewModel)
        {
            InitializeComponent();
            _viewModel = viewModel;
            BindingContext = _viewModel;
        }

        protected override async void OnAppearing()
        {
            base.OnAppearing();
            await _viewModel.LoadUserAndTasksCommand.ExecuteAsync(null);
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
                ((CollectionView)sender).SelectedItem = null;
            }
        }
    }
}