using System;
using System.Threading.Tasks;
using Xunit;
using Moq;
using CheckListApp.ViewModels;
using CheckListApp.Services;

namespace CheckListApp.Tests
{
    public class RegistrationViewModelTests
    {
        private readonly Mock<IAuthenticationService> _authServiceMock;
        private readonly Mock<IPasswordHasher> _passwordHasherMock;
        private readonly RegistrationViewModel _viewModel;

        public RegistrationViewModelTests()
        {
            _authServiceMock = new Mock<IAuthenticationService>();
            _passwordHasherMock = new Mock<IPasswordHasher>();
            _viewModel = new RegistrationViewModel(_authServiceMock.Object, _passwordHasherMock.Object);
        }

        [Fact]
        public void Username_Should_Update_Indicator_When_Set()
        {
            // Arrange
            var username = "TestUser";

            // Act
            _viewModel.Username = username;

            // Assert
            Assert.Equal("✓", _viewModel.UsernameIndicator);
        }

        [Fact]
        public void Password_Should_Update_Indicators_And_Validate_Requirements()
        {
            // Arrange
            var password = "Password1!";

            // Act
            _viewModel.Password = password;

            // Assert
            Assert.Equal("✓", _viewModel.PasswordIndicator);
            Assert.Contains("✓ At least 8 characters", _viewModel.PasswordRequirements);
            Assert.Contains("✓ One uppercase letter", _viewModel.PasswordRequirements);
            Assert.Contains("✓ One number", _viewModel.PasswordRequirements);
        }

        [Fact]
        public void ConfirmPassword_Should_Update_Indicator_When_Matching()
        {
            // Arrange
            _viewModel.Password = "Password1!";
            _viewModel.ConfirmPassword = "Password1!";

            // Act
            var indicator = _viewModel.ConfirmPasswordIndicator;

            // Assert
            Assert.Equal("✓", indicator);
        }

        [Fact]
        public void CanRegister_Should_Return_False_If_Validation_Fails()
        {
            // Arrange
            _viewModel.Username = "TestUser";
            _viewModel.Password = "Pass1!";
            _viewModel.ConfirmPassword = "Pass1!";
            _viewModel.Email = "invalidEmail";
            _viewModel.FirstName = "Test";
            _viewModel.LastName = "User";

            // Act
            var canRegister = _viewModel.RegisterCommand.CanExecute(null);

            // Assert
            Assert.False(canRegister);
        }

        [Fact]
        public async Task RegisterUser_Should_Invoke_AuthService_On_Success()
        {
            // Arrange
            _authServiceMock
                .Setup(x => x.RegisterAsync(It.IsAny<string>(), It.IsAny<string>(), It.IsAny<string>(), It.IsAny<string>(), It.IsAny<string>()))
                .ReturnsAsync((true, "Registration successful!"));

            _viewModel.Username = "TestUser";
            _viewModel.Password = "Password1!";
            _viewModel.ConfirmPassword = "Password1!";
            _viewModel.Email = "test@example.com";
            _viewModel.FirstName = "Test";
            _viewModel.LastName = "User";

            // Act
            await _viewModel.RegisterUser();

            // Assert
            _authServiceMock.Verify(x => x.RegisterAsync(
                "TestUser", "Password1!", "test@example.com", "Test", "User"), Times.Once);
            Assert.Equal("Registration successful! You can now login.", _viewModel.StatusMessage);
            Assert.Equal("Green", _viewModel.StatusMessageColor);
        }

        [Fact]
        public async Task RegisterUser_Should_Set_ErrorStatus_On_Failure()
        {
            // Arrange
            _authServiceMock
                .Setup(x => x.RegisterAsync(It.IsAny<string>(), It.IsAny<string>(), It.IsAny<string>(), It.IsAny<string>(), It.IsAny<string>()))
                .ReturnsAsync((false, "Registration failed."));

            _viewModel.Username = "TestUser";
            _viewModel.Password = "Password1!";
            _viewModel.ConfirmPassword = "Password1!";
            _viewModel.Email = "test@example.com";
            _viewModel.FirstName = "Test";
            _viewModel.LastName = "User";

            // Act
            await _viewModel.RegisterUser();

            // Assert
            Assert.Equal("Registration failed.", _viewModel.StatusMessage);
            Assert.Equal("Red", _viewModel.StatusMessageColor);
        }

        [Fact]
        public void RegisterCommandShouldInvokeAuthServiceOnSuccess()
        {
            // Arrange
            _authServiceMock
    .Setup(x => x.RegisterAsync(
        It.IsAny<string>(),
        It.IsAny<string>(),
        It.IsAny<string>(),
        It.IsAny<string>(),
        It.IsAny<string>()))
    .ReturnsAsync((true, "Registration successful!"));


            _viewModel.Username = "TestUser";
            _viewModel.Password = "Password1!";
            _viewModel.ConfirmPassword = "Password1!";
            _viewModel.Email = "test@example.com";
            _viewModel.FirstName = "Test";
            _viewModel.LastName = "User";

            // Act
            _authServiceMock
    .Setup(x => x.RegisterAsync(It.IsAny<string>(), It.IsAny<string>(), It.IsAny<string>(), It.IsAny<string>(), It.IsAny<string>()))
    .ReturnsAsync((true, "Registration successful!"));



            // Assert
            _authServiceMock.Verify(x => x.RegisterAsync(
                "TestUser", "Password1!", "test@example.com", "Test", "User"), Times.Once);
            Assert.Equal("Registration successful! You can now login.", _viewModel.StatusMessage);
            Assert.Equal("Green", _viewModel.StatusMessageColor);
        }

    }
}
