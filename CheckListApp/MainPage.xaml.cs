﻿using CheckListApp.ViewModels;

namespace CheckListApp
{
    public partial class MainPage : ContentPage
    {
        public MainPage()
        {
        }

        public MainPage(MainPageViewModel viewModel)
        {
            InitializeComponent();
            BindingContext = viewModel;
        }
    }
}