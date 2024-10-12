using Microsoft.Maui.Controls;
using CheckListApp.Services;
using CheckListApp.Model;

namespace CheckListApp.View
{
    public partial class ItemDetailPage : ContentPage
    {
        private readonly UserTaskService _userTaskService;
        private UserTask _currentTask;

        public int TaskId { get; set; }
        public int UserId { get; set; }

        public ItemDetailPage()
        {
            InitializeComponent();
           
        }
    }
}