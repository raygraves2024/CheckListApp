using System.Threading.Tasks;
using Xunit;
using Moq;
using CheckListApp.ViewModels;
using CheckListApp.Services;
using CommunityToolkit.Mvvm.Input;

namespace checklistapptest
{
    public class LoginViewModelTests
    {
        private readonly Mock<IAuthenticationService> _authServiceMock;
        private readonly Mock<IPasswordHasher> _passwordHasherMock;
        private readonly LoginViewModel _viewModel;

        public LoginViewModelTests()
        {
            _authServiceMock = new Mock<IAuthenticationService>();
            _passwordHasherMock = new Mock<IPasswordHasher>();

            _viewModel = new LoginViewModel(_authServiceMock.Object, _passwordHasherMock.Object);
        }

        [Fact]
        public void LoginCommand_CannotExecute_WhenFieldsAreEmpty()
        {
            // Arrange
            _viewModel.Username = "";
            _viewModel.Password = "";

            // Act
            var canExecute = _viewModel.LoginCommand.CanExecute(null);

            // Assert
            Assert.False(canExecute);
        }

        [Fact]
        public void LoginCommand_CanExecute_WhenFieldsAreFilled()
        {
            // Arrange
            _viewModel.Username = "testuser";
            _viewModel.Password = "password123";

            // Act
            var canExecute = _viewModel.LoginCommand.CanExecute(null);

            // Assert
            Assert.True(canExecute);
        }

        [Fact]
        public async Task OnLoginClicked_ShouldCallLoginAsync_WhenExecuted()
        {
            // Arrange
            _viewModel.Username = "testuser";
            _viewModel.Password = "password123";
            _authServiceMock
                .Setup(s => s.LoginAsync(It.IsAny<string>(), It.IsAny<string>()))
                .ReturnsAsync((true, "Login successful"));

            // Act
            await ((AsyncRelayCommand)_viewModel.LoginCommand).ExecuteAsync(null);

            // Assert
            _authServiceMock.Verify(s => s.LoginAsync("testuser", "password123"), Times.Once);
            Assert.Equal("", _viewModel.Password); // Ensure password is cleared after login
        }

        [Fact]
        public async Task OnLoginClicked_ShouldSetErrorMessage_OnFailure()
        {
            // Arrange
            _viewModel.Username = "testuser";
            _viewModel.Password = "wrongpassword";
            _authServiceMock
                .Setup(s => s.LoginAsync(It.IsAny<string>(), It.IsAny<string>()))
                .ReturnsAsync((false, "Invalid credentials"));

            // Act
            await ((AsyncRelayCommand)_viewModel.LoginCommand).ExecuteAsync(null);

            // Assert
            Assert.Equal("Invalid credentials", _viewModel.ErrorMessage);
        }

        [Fact]
        public void ResetState_ShouldClearProperties()
        {
            // Arrange
            _viewModel.Username = "testuser";
            _viewModel.Password = "password123";
            _viewModel.ErrorMessage = "Some error";
            _viewModel.IsLoading = true;

            // Act
            _viewModel.ResetState();

            // Assert
            Assert.Equal(string.Empty, _viewModel.Username);
            Assert.Equal(string.Empty, _viewModel.Password);
            Assert.Equal(string.Empty, _viewModel.ErrorMessage);
            Assert.False(_viewModel.IsLoading);
        }
    }
}
