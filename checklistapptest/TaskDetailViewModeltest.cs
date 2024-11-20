using Xunit;
using Moq;
using System.Threading.Tasks;
using CheckListApp.ViewModels;
using CheckListApp.Model;
using CheckListApp.Services;
using System.Collections.Generic;

public class TaskDetailViewModelTests
{
    [Fact]
    public void LoadTasks_ShouldLoadTasksSuccessfully()
    {
        // Arrange
        var mockUserTaskService = new Mock<UserTaskService>();
        var testTasks = new List<UserTask>
        {
            new UserTask { TaskID = 1, Title = "Task 1", IsCompleted = false },
            new UserTask { TaskID = 2, Title = "Task 2", IsCompleted = true }
        };

        mockUserTaskService
            .Setup(service => service.GetTasksAsync(It.IsAny<int>()))
            .ReturnsAsync(testTasks);

        var viewModel = new TaskDetailViewModel(mockUserTaskService.Object);

        // Act
        viewModel.LoadTasksCommand.Execute(null);

        // Assert
        Assert.Equal(2, viewModel.Tasks.Count);
        Assert.Contains(viewModel.Tasks, t => t.Title == "Task 1");
        Assert.Contains(viewModel.Tasks, t => t.Title == "Task 2");
    }
}


