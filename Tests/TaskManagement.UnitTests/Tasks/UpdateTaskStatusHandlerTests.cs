using Application.Features.Tasks.Commands.UpdateStatus;
using Application.Common.ResultPattern;
using Moq;
using TaskManagement.UnitTests.Common;
using DomainTask = Domain.Entities.Task;
using TaskStatus = Domain.Entities.Enums.TaskStatus;

namespace TaskManagement.UnitTests.Tasks;

public class UpdateTaskStatusHandlerTests : HandlerTestBase
{
    private UpdateStatusCommandHandler CreateHandler() =>
        new(UnitOfWork.Object, UserService.Object, Authorization.Object);

    private void TaskExists(DomainTask task) =>
        TaskRepo.Setup(r => r.GetTaskByIdAsync(task.Id, It.IsAny<CancellationToken>())).ReturnsAsync(task);

    // ---------- success path ----------

    [Fact]
    public async Task Handle_ValidTransition_ChangesStatusAndSaves()
    {
        // Arrange
        var task = TestData.CreateTask(TaskStatus.Todo);
        SignInAs(TestData.OwnerId);
        TaskExists(task);
        AuthorizationSucceeds();

        // Act
        var result = await CreateHandler().Handle(
            new UpdateStatusCommand(task.Id, TaskStatus.InProgress), CancellationToken.None);

        // Assert
        Assert.True(result.IsSuccess);
        Assert.Equal(TaskStatus.InProgress, result.Value);
        Assert.Equal(TaskStatus.InProgress, task.Status);
        Assert.NotNull(task.UpdatedAt);
        UnitOfWork.Verify(u => u.SaveChangesAsync(It.IsAny<CancellationToken>()), Times.Once());
    }

    // ---------- failure paths ----------

    [Fact]
    public async Task Handle_InvalidTransition_ReturnsConflictAndDoesNotSave()
    {
// Arrange: jumping straight from the initial status to Completed is not allowed (must go through InProgress)
        var task = TestData.CreateTask(TaskStatus.Todo);
        SignInAs(TestData.OwnerId);
        TaskExists(task);
        AuthorizationSucceeds();

        // Act
        var result = await CreateHandler().Handle(
            new UpdateStatusCommand(task.Id, TaskStatus.Completed), CancellationToken.None);

        // Assert
        AssertFailure(result, "Task.InvalidStatusTransition", ErrorType.Conflict);
        Assert.Equal(TaskStatus.Todo, task.Status);
        VerifyNothingWasSaved();
    }

    [Fact]
    public async Task Handle_TaskDoesNotExist_ReturnsNotFound()
    {
        // Arrange
        SignInAs(TestData.OwnerId);
        TaskRepo.Setup(r => r.GetTaskByIdAsync(999, It.IsAny<CancellationToken>()))
            .ReturnsAsync((DomainTask?)null);

        // Act
        var result = await CreateHandler().Handle(
            new UpdateStatusCommand(999, TaskStatus.InProgress), CancellationToken.None);

        // Assert
        AssertFailure(result, "Task.NotFound", ErrorType.NotFound);
        VerifyNothingWasSaved();
    }

    [Fact]
    public async Task Handle_UserNotAuthenticated_ReturnsUnauthorized()
    {
        // Arrange: no SignInAs(...) => nobody is logged in
        var task = TestData.CreateTask();
        TaskExists(task);

        // Act
        var result = await CreateHandler().Handle(
            new UpdateStatusCommand(task.Id, TaskStatus.InProgress), CancellationToken.None);

        // Assert
        AssertFailure(result, "User.NotAuthenticated", ErrorType.Unauthorized);
        Assert.Equal(TaskStatus.Todo, task.Status);
        VerifyNothingWasSaved();
    }

    [Fact]
    public async Task Handle_UserIsNotTheOwner_ReturnsForbidden()
    {
        // Arrange
        var task = TestData.CreateTask(createdByUserId: TestData.OwnerId);
        SignInAs(TestData.OtherUserId);
        TaskExists(task);
        AuthorizationFails();

        // Act
        var result = await CreateHandler().Handle(
            new UpdateStatusCommand(task.Id, TaskStatus.InProgress), CancellationToken.None);

        // Assert
        AssertFailure(result, "Task.Forbidden", ErrorType.Forbidden);
        Assert.Equal(TaskStatus.Todo, task.Status);
        VerifyNothingWasSaved();
    }
}
