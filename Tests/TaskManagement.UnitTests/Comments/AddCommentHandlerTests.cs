using Application.Common.ResultPattern;
using Application.Features.Comments.Commands.Add;
using Domain.Entities.Enums;
using Moq;
using TaskManagement.UnitTests.Common;
using Comment = Domain.Entities.Comment;
using DomainTask = Domain.Entities.Task;
using Project = Domain.Entities.Project;
using TaskStatus = Domain.Entities.Enums.TaskStatus;

namespace TaskManagement.UnitTests.Comments;

public class AddCommentHandlerTests : HandlerTestBase
{
    private const string ProjectOwnerId = "project-owner-id";

    // AddCommentCommandHandler doesn't use IAuthorizationService, so we simply don't pass it.
    private AddCommentCommandHandler CreateHandler() => new(UnitOfWork.Object, UserService.Object);

    private void TaskExists(DomainTask task) =>
        TaskRepo.Setup(r => r.GetTaskByIdAsync(task.Id, It.IsAny<CancellationToken>())).ReturnsAsync(task);

    private void ProjectExists(Project project) =>
        ProjectRepo.Setup(r => r.GetByIdAsync(project.Id, It.IsAny<CancellationToken>())).ReturnsAsync(project);

    private void NoCommentWasAdded() =>
        CommentRepo.Verify(r => r.AddAsync(It.IsAny<Comment>(), It.IsAny<CancellationToken>()), Times.Never());

    // ---------- success paths ----------

    [Fact]
    public async Task Handle_TaskOwnerComments_AddsCommentToTask()
    {
        // Arrange
        var task = TestData.CreateTask(createdByUserId: TestData.OwnerId);
        var project = TestData.CreateProject(id: task.ProjectId, createdByUserId: ProjectOwnerId);
        TaskExists(task);
        ProjectExists(project);
        SignInAs(TestData.OwnerId);

        Comment? added = null;
        CommentRepo.Setup(r => r.AddAsync(It.IsAny<Comment>(), It.IsAny<CancellationToken>()))
            .Callback<Comment, CancellationToken>((comment, _) => { added = comment; comment.Id = 7; })
            .Returns(Task.CompletedTask);

        // Act
        var result = await CreateHandler().Handle(
            new AddCommentCommand("Looks good", task.Id), CancellationToken.None);

        // Assert
        Assert.True(result.IsSuccess);
        Assert.Equal(7, result.Value);
        Assert.NotNull(added);
        Assert.Equal("Looks good", added!.Content);
        Assert.Equal(task.Id, added.TaskId);
        Assert.Equal(TestData.OwnerId, added.CreatedByUserId);
        UnitOfWork.Verify(u => u.SaveChangesAsync(It.IsAny<CancellationToken>()), Times.Once());
    }

    [Fact]
    public async Task Handle_ProjectOwnerComments_IsAllowedEvenIfNotTheTaskOwner()
    {
        // Arrange
        var task = TestData.CreateTask(createdByUserId: TestData.OwnerId);
        var project = TestData.CreateProject(id: task.ProjectId, createdByUserId: ProjectOwnerId);
        TaskExists(task);
        ProjectExists(project);
        SignInAs(ProjectOwnerId);

        // Act
        var result = await CreateHandler().Handle(
            new AddCommentCommand("Please update", task.Id), CancellationToken.None);

        // Assert
        Assert.True(result.IsSuccess);
        CommentRepo.Verify(r => r.AddAsync(
            It.Is<Comment>(c => c.CreatedByUserId == ProjectOwnerId && c.TaskId == task.Id),
            It.IsAny<CancellationToken>()), Times.Once());
        UnitOfWork.Verify(u => u.SaveChangesAsync(It.IsAny<CancellationToken>()), Times.Once());
    }

    // ---------- failure paths ----------

    [Fact]
    public async Task Handle_TaskDoesNotExist_ReturnsNotFoundAndAddsNothing()
    {
        // Arrange
        SignInAs(TestData.OwnerId);
        TaskRepo.Setup(r => r.GetTaskByIdAsync(999, It.IsAny<CancellationToken>()))
            .ReturnsAsync((DomainTask?)null);

        // Act
        var result = await CreateHandler().Handle(
            new AddCommentCommand("Hello", 999), CancellationToken.None);

        // Assert
        AssertFailure(result, "Task.NotFound", ErrorType.NotFound);
        NoCommentWasAdded();
        VerifyNothingWasSaved();
    }

    [Theory]
    [InlineData(TaskStatus.Completed)]
    [InlineData(TaskStatus.Cancelled)]
    public async Task Handle_TaskIsClosed_ReturnsConflictAndAddsNothing(TaskStatus closedStatus)
    {
        // Arrange
        var task = TestData.CreateTask(closedStatus);
        TaskExists(task);
        SignInAs(TestData.OwnerId);

        // Act
        var result = await CreateHandler().Handle(
            new AddCommentCommand("Too late", task.Id), CancellationToken.None);

        // Assert
        AssertFailure(result, "Task.AlreadyClosed", ErrorType.Conflict);
        NoCommentWasAdded();
        VerifyNothingWasSaved();
    }

    [Fact]
    public async Task Handle_UserNotAuthenticated_ReturnsUnauthorizedAndAddsNothing()
    {
        // Arrange: no SignInAs(...) => nobody is logged in
        var task = TestData.CreateTask();
        TaskExists(task);

        // Act
        var result = await CreateHandler().Handle(
            new AddCommentCommand("Hello", task.Id), CancellationToken.None);

        // Assert
        AssertFailure(result, "User.NotAuthenticated", ErrorType.Unauthorized);
        NoCommentWasAdded();
        VerifyNothingWasSaved();
    }

    [Fact]
    public async Task Handle_ParentProjectDoesNotExist_ReturnsNotFoundAndAddsNothing()
    {
        // Arrange
        var task = TestData.CreateTask();
        TaskExists(task);
        SignInAs(TestData.OwnerId);
        ProjectRepo.Setup(r => r.GetByIdAsync(task.ProjectId, It.IsAny<CancellationToken>()))
            .ReturnsAsync((Project?)null);

        // Act
        var result = await CreateHandler().Handle(
            new AddCommentCommand("Hello", task.Id), CancellationToken.None);

        // Assert
        AssertFailure(result, "Project.NotFound", ErrorType.NotFound);
        NoCommentWasAdded();
        VerifyNothingWasSaved();
    }

    [Fact]
    public async Task Handle_UserOwnsNeitherTaskNorProject_ReturnsForbiddenAndAddsNothing()
    {
        // Arrange
        var task = TestData.CreateTask(createdByUserId: TestData.OwnerId);
        var project = TestData.CreateProject(id: task.ProjectId, createdByUserId: ProjectOwnerId);
        TaskExists(task);
        ProjectExists(project);
        SignInAs(TestData.OtherUserId);

        // Act
        var result = await CreateHandler().Handle(
            new AddCommentCommand("Can I join?", task.Id), CancellationToken.None);

        // Assert
        AssertFailure(result, "Comment.Forbidden", ErrorType.Forbidden);
        NoCommentWasAdded();
        VerifyNothingWasSaved();
    }
}
