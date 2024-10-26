using Microsoft.Maui.Controls;

namespace YourAppNamespace
{
    public partial class LoginPage : ContentPage
    {
        // Define a hard-coded username and password
        private const string CorrectUsername = "admin";
        private const string CorrectPassword = "password123";

        public LoginPage()
        {
            InitializeComponent();
        }

        private void OnLoginButtonClicked(object sender, EventArgs e)
        {
            // Get the input from the user
            string username = UsernameEntry.Text;
            string password = PasswordEntry.Text;

            // Check if the credentials match
            if (username == CorrectUsername && password == CorrectPassword)
            {
                StatusLabel.IsVisible = false; // Hide any previous error messages
                // Navigate to the main checklist page
                Application.Current.MainPage = new MainPage();
            }
            else
            {
                // Show an error message
                StatusLabel.Text = "Invalid username or password. Please try again.";
                StatusLabel.IsVisible = true;
            }
        }
    }
}

