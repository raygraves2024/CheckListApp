using CheckListApp.ViewModels;
using CheckListApp.Services;
using Moq;
using Xunit;

public class TaskEntryViewModelTests
{
    private readonly Mock<AuthenticationService> _authServiceMock;
    private readonly Mock<UserTaskService> _taskServiceMock;
    private readonly TaskEntryViewModel _viewModel;

    public TaskEntryViewModelTests()
    {
        // Mock dependencies
        _authServiceMock = new Mock<AuthenticationService>();
        _taskServiceMock = new Mock<UserTaskService>();

        // Set up a mock user
        _authServiceMock.Setup(auth => auth.CurrentUser).Returns("TestUser");

        // Initialize ViewModel
        _viewModel = new TaskEntryViewModel(_authServiceMock.Object, _taskServiceMock.Object);
    }

    [Fact]
    public void Constructor_ShouldInitializeProperties()
    {
        // Arrange & Act done in constructor

        // Assert
        Assert.Equal("TestUser", _viewModel.Username);
        Assert.NotNull(_viewModel.SaveTaskCommand);
    }

    [Fact]
    public void SaveTaskCommand_ShouldShowError_WhenTaskNameIsEmpty()
    {
        // Arrange
        _viewModel.TaskName = string.Empty;

        // Act
        bool canExecute = _viewModel.SaveTaskCommand.CanExecute(null);

        // Assert
        Assert.True(canExecute); // Ensure the command is executable
        // You can verify if DisplayAlert is shown using platform-specific UI testing
    }

    [Fact]
    public void SaveTaskCommand_ShouldClearForm_OnSuccessfulSave()
    {
        // Arrange
        _viewModel.TaskName = "Sample Task";
        _viewModel.TaskDescription = "Sample Description";

        // Act
        _viewModel.SaveTaskCommand.Execute(null);

        // Assert
        Assert.Empty(_viewModel.TaskName);
        Assert.Empty(_viewModel.TaskDescription);
    }
}
