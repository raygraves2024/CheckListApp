using CheckListApp.Model;
using CheckListApp.Services;
using CheckListApp.ViewModels;
using Moq;
using System;
using System.Collections.ObjectModel;
using System.Threading.Tasks;
using Xunit;

namespace CheckListApp.Tests.ViewModels
{
    public class MainPageViewModelTests
    {
        private readonly Mock<AuthenticationService> _mockAuthService;
        private readonly Mock<UserTaskService> _mockUserTaskService;
        private readonly MainPageViewModel _viewModel;

        public MainPageViewModelTests()
        {
            _mockAuthService = new Mock<AuthenticationService>();
            _mockUserTaskService = new Mock<UserTaskService>();

            _viewModel = new MainPageViewModel(_mockAuthService.Object, _mockUserTaskService.Object);
        }

        [Fact]
        public async Task LoginAsync_SetsUserTasks_WhenSuccessful()
        {
            // Arrange
            var mockTasks = new List<UserTask>
{
    new UserTask { TaskID = 1, Title = "Test Task 1" },
    new UserTask { TaskID = 2, Title = "Test Task 2" }
};

            _mockUserTaskService
                .Setup(s => s.GetTasksForUserAsync(1))
                .ReturnsAsync(mockTasks);
            // Act
            await _viewModel.LoginCommand.ExecuteAsync(null);

            // Assert
            Assert.Equal(2, _viewModel.UserTasks.Count);
            Assert.Contains(_viewModel.UserTasks, t => t.TaskID == 1);
            Assert.Contains(_viewModel.UserTasks, t => t.TaskID == 2);
        }

        [Fact]
        public async Task LoginAsync_SetsErrorMessage_WhenExceptionOccurs()
        {
            // Arrange
            var exceptionMessage = "Failed to load tasks";
            _mockUserTaskService
                .Setup(s => s.GetTasksForUserAsync(1))
                .ThrowsAsync(new Exception(exceptionMessage));

            // Act
            await _viewModel.LoginCommand.ExecuteAsync(null);

            // Assert
            Assert.Equal($"Error loading tasks: {exceptionMessage}", _viewModel.ErrorMessage);
            Assert.Empty(_viewModel.UserTasks);
        }

        [Fact]
        public async Task LoginAsync_SetsIsLoadingProperly()
        {
            // Arrange
            _mockUserTaskService
                .Setup(s => s.GetTasksForUserAsync(1))
                .ReturnsAsync(new List<UserTask>());

            // Act
            var task = _viewModel.LoginCommand.ExecuteAsync(null);

            // Assert
            Assert.True(_viewModel.IsLoading);

            await task;

            Assert.False(_viewModel.IsLoading);
        }

    }
}
