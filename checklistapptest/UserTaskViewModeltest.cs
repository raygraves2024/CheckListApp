using Moq;
using Xunit;
using CheckListApp.ViewModels;
using CheckListApp.Services;
using CheckListApp.Model;
using System.Collections.ObjectModel;
using System.Threading.Tasks;

public class UserTaskViewModelTests
{
    private readonly Mock<UserTaskService> _mockTaskService;
    private readonly Mock<UserService> _mockUserService;
    private readonly UserTaskViewModel _viewModel;

    public UserTaskViewModelTests()
    {
        _mockTaskService = new Mock<UserTaskService>();
        _mockUserService = new Mock<UserService>();
        _viewModel = new UserTaskViewModel(_mockTaskService.Object, _mockUserService.Object);
    }

    [Fact]
    public async Task LoadUserAndTasksCommand_ShouldLoadUserAndTasks()
    {
        // Arrange
        var mockUser = new Users { UserID = 1, FirstName = "John", LastName = "Doe" };
        var mockTasks = new List<UserTask>
    {
        new UserTask { TaskID = 1, Title = "Task 1", IsCompleted = false, PriorityLevel = 1 },
        new UserTask { TaskID = 2, Title = "Task 2", IsCompleted = true, PriorityLevel = 2 }
    };

        _mockUserService.Setup(service => service.GetFirstUserAsync()).ReturnsAsync(mockUser);
        _mockTaskService.Setup(service => service.GetTasksForUserAsync(mockUser.UserID)).ReturnsAsync(mockTasks);

        // Act
        await _viewModel.LoadUserAndTasksCommand.ExecuteAsync(null);

        // Assert
        Assert.Equal("John Doe", _viewModel.UserFullName);
        Assert.Equal(2, _viewModel.UserTasks.Count);
        Assert.False(_viewModel.IsLoading);
        Assert.Empty(_viewModel.ErrorMessage);
    }




    [Fact]
    public async Task SelectTaskCommand_ShouldSetSelectedTask()
    {
        // Arrange
        var mockTask = new UserTask { TaskID = 1, Title = "Task 1" };

        // Act
        await _viewModel.SelectTaskCommand.ExecuteAsync(mockTask);

        // Assert
        Assert.Equal(mockTask, _viewModel.SelectedTask);
    }


    [Fact]
    public async Task DeleteTaskAsync_WithValidTask_ShouldRemoveTaskFromCollection()
    {
        // Arrange
        var mockTask = new UserTask { TaskID = 1, Title = "Task 1" };
        _viewModel.UserTasks.Add(mockTask);

        // Mock DeleteTaskAsync to return a Task<bool>
        _mockTaskService.Setup(service => service.DeleteTaskAsync(mockTask.TaskID)).ReturnsAsync(true);

        // Act
        await _viewModel.DeleteTaskAsync(mockTask);

        // Assert
        Assert.DoesNotContain(mockTask, _viewModel.UserTasks);
    }



    [Fact]
    public async Task ToggleTaskCompletionCommand_ShouldUpdateTaskCompletionStatus()
    {
        // Arrange
        var mockTask = new UserTask { TaskID = 1, Title = "Task 1", IsCompleted = false };
        _viewModel.UserTasks.Add(mockTask);
        _mockTaskService.Setup(service => service.UpdateTaskAsync(mockTask)).ReturnsAsync(true);

        // Act
        await _viewModel.ToggleTaskCompletionCommand.ExecuteAsync(mockTask);

        // Assert
        Assert.True(mockTask.IsCompleted);
        Assert.Equal(1, _viewModel.UserTasks.Count);
    }


}