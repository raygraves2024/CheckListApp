namespace CheckListApp.View
{
    public partial class ErrorPage : ContentPage, IQueryAttributable
    {
        public static readonly BindableProperty ErrorMessageProperty =
            BindableProperty.Create(nameof(ErrorMessage), typeof(string), typeof(ErrorPage), string.Empty);

        public string ErrorMessage
        {
            get => (string)GetValue(ErrorMessageProperty);
            set => SetValue(ErrorMessageProperty, value);
        }

        public ErrorPage()
        {
            InitializeComponent();
            BindingContext = this;
        }

        public ErrorPage(string errorMessage) : this()
        {
            ErrorMessage = errorMessage;
        }

        public void ApplyQueryAttributes(IDictionary<string, object> query)
        {
            if (query.ContainsKey("message"))
            {
                ErrorMessage = Uri.UnescapeDataString(query["message"].ToString());
            }
        }

        private async void OnBackToLoginClicked(object sender, EventArgs e)
        {
            try
            {
                // Disable the button to prevent multiple clicks
                if (sender is Button button)
                {
                    button.IsEnabled = false;
                }

                // Clear navigation stack and go back to login
                await Shell.Current.GoToAsync("//LoginPage", animate: true);
            }
            catch (Exception ex)
            {
                await DisplayAlert("Navigation Error",
                    "Unable to return to login page. Please restart the application.", "OK");
                System.Diagnostics.Debug.WriteLine($"Navigation error: {ex.Message}");
            }
            finally
            {
                // Re-enable the button
                if (sender is Button button)
                {
                    button.IsEnabled = true;
                }
            }
        }

        protected override bool OnBackButtonPressed()
        {
            // Handle the back button press to ensure proper navigation
            MainThread.BeginInvokeOnMainThread(async () =>
            {
                await Shell.Current.GoToAsync("//LoginPage");
            });
            return true;
        }
    }
}