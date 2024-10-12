using CheckListApp.ViewModels;

namespace CheckListApp.View
{
    public partial class TaskEntryPage : ContentPage
    {
        public TaskEntryPage(TaskEntryViewModel viewModel)
        {
            InitializeComponent();
            BindingContext = viewModel;
        }
    }
}