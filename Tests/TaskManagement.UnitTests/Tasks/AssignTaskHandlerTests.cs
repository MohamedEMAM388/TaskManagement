using Application.Common.Identity;
using Application.Common.ResultPattern;
using Application.Contracts;
using Application.Features.Tasks.Commands.Assign;
using Moq;
using TaskManagement.UnitTests.Common;
using DomainTask = Domain.Entities.Task;
using TaskStatus = Domain.Entities.Enums.TaskStatus;

namespace TaskManagement.UnitTests.Tasks;

public class AssignTaskHandlerTests : HandlerTestBase
{
    private const string AssigneeId = "assignee-user-id";

    // IIdentityService is an external collaborator (ASP.NET Identity + a separate database), so it is mocked.
    private readonly Mock<IIdentityService> _identityService = new();

    private AssignTaskCommandHandler CreateHandler() =>
        new(UnitOfWork.Object, UserService.Object, _identityService.Object, Authorization.Object);

    private void TaskExists(DomainTask task) =>
        TaskRepo.Setup(r => r.GetTaskByIdAsync(task.Id, It.IsAny<CancellationToken>())).ReturnsAsync(task);

    private void AssigneeExists(string userId) =>
        _identityService.Setup(i => i.GetUserByIdAsync(userId))
            .ReturnsAsync(Result<IdentityUserResult>.Ok(
                new IdentityUserResult(userId, "assignee@test.com", "Assignee", "assignee")));

    private void AssigneeDoesNotExist(string userId) =>
        _identityService.Setup(i => i.GetUserByIdAsync(userId))
            .ReturnsAsync(Result<IdentityUserResult>.Fail(
                Error.NotFound("User.NotFound", $"User with id '{userId}' was not found.")));

    private void NoUserLookupHappened() =>
        _identityService.Verify(i => i.GetUserByIdAsync(It.IsAny<string>()), Times.Never());

    // ---------- success paths ----------

    [Fact]
    public async Task Handle_ExistingUser_AssignsTaskAndSaves()
    {
        // Arrange
        var task = TestData.CreateTask(TaskStatus.InProgress);
        SignInAs(TestData.OwnerId);
        TaskExists(task);
        AuthorizationSucceeds();
        AssigneeExists(AssigneeId);

        // Act
        var result = await CreateHandler().Handle(
            new AssignTaskCommand(task.Id, AssigneeId), CancellationToken.None);

        // Assert
        Assert.True(result.IsSuccess);
        Assert.Equal(AssigneeId, task.AssigneeId);
        Assert.NotNull(task.UpdatedAt);
        UnitOfWork.Verify(u => u.SaveChangesAsync(It.IsAny<CancellationToken>()), Times.Once());
    }

    [Fact]
    public async Task Handle_TaskAlreadyAssigned_ReassignsToNewUser()
    {
        // Arrange
        var task = TestData.CreateTask();
        task.AssignTo("previous-assignee");
        SignInAs(TestData.OwnerId);
        TaskExists(task);
        AuthorizationSucceeds();
        AssigneeExists(AssigneeId);

        // Act
        var result = await CreateHandler().Handle(
            new AssignTaskCommand(task.Id, AssigneeId), CancellationToken.None);

        // Assert
        Assert.True(result.IsSuccess);
        Assert.Equal(AssigneeId, task.AssigneeId);
    }

    // ---------- failure paths ----------

    [Fact]
    public async Task Handle_UserDoesNotExist_ReturnsNotFoundAndLeavesTaskUnassigned()
    {
        // Arrange
        var task = TestData.CreateTask();
        SignInAs(TestData.OwnerId);
        TaskExists(task);
        AuthorizationSucceeds();
        AssigneeDoesNotExist("ghost-user");

        // Act
        var result = await CreateHandler().Handle(
            new AssignTaskCommand(task.Id, "ghost-user"), CancellationToken.None);

        // Assert
        AssertFailure(result, "User.NotFound", ErrorType.NotFound);
        Assert.Null(task.AssigneeId);
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
            new AssignTaskCommand(999, AssigneeId), CancellationToken.None);

        // Assert
        AssertFailure(result, "Task.NotFound", ErrorType.NotFound);
        NoUserLookupHappened();
        VerifyNothingWasSaved();
    }

    [Theory]
    [InlineData(TaskStatus.Completed)]
    [InlineData(TaskStatus.Cancelled)]
    public async Task Handle_TaskIsClosed_ReturnsConflictAndLeavesTaskUnassigned(TaskStatus closedStatus)
    {
        // Arrange
        var task = TestData.CreateTask(closedStatus);
        SignInAs(TestData.OwnerId);
        TaskExists(task);
        AuthorizationSucceeds();
        AssigneeExists(AssigneeId);

        // Act
        var result = await CreateHandler().Handle(
            new AssignTaskCommand(task.Id, AssigneeId), CancellationToken.None);

        // Assert
        AssertFailure(result, "Task.AlreadyClosed", ErrorType.Conflict);
        Assert.Null(task.AssigneeId);
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
            new AssignTaskCommand(task.Id, AssigneeId), CancellationToken.None);

        // Assert
        AssertFailure(result, "User.NotAuthenticated", ErrorType.Unauthorized);
        Assert.Null(task.AssigneeId);
        NoUserLookupHappened();
        VerifyNothingWasSaved();
    }

    [Fact]
    public async Task Handle_CallerIsNotTheTaskOwner_ReturnsForbidden()
    {
        // Arrange
        var task = TestData.CreateTask(createdByUserId: TestData.OwnerId);
        SignInAs(TestData.OtherUserId);
        TaskExists(task);
        AuthorizationFails();
        AssigneeExists(AssigneeId);

        // Act
        var result = await CreateHandler().Handle(
            new AssignTaskCommand(task.Id, AssigneeId), CancellationToken.None);

        // Assert
        AssertFailure(result, "Task.Forbidden", ErrorType.Forbidden);
        Assert.Null(task.AssigneeId);
        NoUserLookupHappened();
        VerifyNothingWasSaved();
    }
}
