namespace CheckListApp.Services
{
    public class AuthenticationService
    {
        private bool _isAuthenticated = false;
        private string _currentUser = string.Empty;

        public bool IsAuthenticated => _isAuthenticated;
        public string CurrentUser => _currentUser;

        public bool Login(string username, string password)
        {
            // TODO: Implement actual authentication logic here
            // For now, we'll use a simple check
            if (!string.IsNullOrEmpty(username) && !string.IsNullOrEmpty(password))
            {
                _isAuthenticated = true;
                _currentUser = username;
                return true;
            }

            return false;
        }

        public void Logout()
        {
            _isAuthenticated = false;
            _currentUser = string.Empty;
        }
    }
}