using CheckListApp.ViewModels;

namespace CheckListApp.View;

public partial class RegistrationPage : ContentPage
{
	public RegistrationPage()
	{
        InitializeComponent();
        BindingContext = new RegistrationViewModel();
    }
}