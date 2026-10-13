using Application.Common.ResultPattern;
using Application.Features.Tasks.Commands.Create;
using Domain.Entities.Enums;
using Moq;
using TaskManagement.UnitTests.Common;
using Project = Domain.Entities.Project;
using DomainTask = Domain.Entities.Task;

namespace TaskManagement.UnitTests.Tasks;

public class CreateTaskHandlerTests : HandlerTestBase
{
    private CreateTaskCommandHandler CreateHandler() =>
        new(UnitOfWork.Object, UserService.Object, Authorization.Object);

    private static CreateTaskCommand CommandFor(int projectId) =>
        new("Write report", "Quarterly report", new DateTime(2030, 1, 1), projectId);

    private void ProjectExists(Project project) =>
        ProjectRepo.Setup(r => r.GetByIdAsync(project.Id, It.IsAny<CancellationToken>())).ReturnsAsync(project);

    private void NoTaskWasCreated() =>
        TaskRepo.Verify(r => r.CreateTaskAsync(It.IsAny<DomainTask>(), It.IsAny<CancellationToken>()), Times.Never());

    // ---------- success path ----------

    [Theory]
    [InlineData(ProjectStatus.Pending)]
    [InlineData(ProjectStatus.Planning)]
    [InlineData(ProjectStatus.InProgress)]
    public async Task Handle_ValidProject_CreatesTaskOwnedByCurrentUser(ProjectStatus projectStatus)
    {
        // Arrange
        var project = TestData.CreateProject(projectStatus);
        ProjectExists(project);
        SignInAs(TestData.OwnerId);
        AuthorizationSucceeds();

        DomainTask? createdTask = null;
        TaskRepo.Setup(r => r.CreateTaskAsync(It.IsAny<DomainTask>(), It.IsAny<CancellationToken>()))
            .Callback<DomainTask, CancellationToken>((task, _) =>
            {
                createdTask = task;
                task.Id = 42; // simulate the id the database would generate
            })
            .Returns(Task.CompletedTask);

        // Act
        var result = await CreateHandler().Handle(CommandFor(project.Id), CancellationToken.None);

        // Assert
        Assert.True(result.IsSuccess);
        Assert.Equal(42, result.Value);
        Assert.NotNull(createdTask);
        Assert.Equal(project.Id, createdTask!.ProjectId);
        Assert.Equal("Write report", createdTask.Title);
        Assert.Equal(TestData.OwnerId, createdTask.CreatedByUserId);
        UnitOfWork.Verify(u => u.SaveChangesAsync(It.IsAny<CancellationToken>()), Times.Once());
    }

    // ---------- failure paths ----------

    [Fact]
    public async Task Handle_ProjectDoesNotExist_ReturnsNotFoundAndCreatesNothing()
    {
        // Arrange
        SignInAs(TestData.OwnerId);
        ProjectRepo.Setup(r => r.GetByIdAsync(999, It.IsAny<CancellationToken>()))
            .ReturnsAsync((Project?)null);

        // Act
        var result = await CreateHandler().Handle(CommandFor(999), CancellationToken.None);

        // Assert
        AssertFailure(result, "Project.NotFound", ErrorType.NotFound);
        NoTaskWasCreated();
        VerifyNothingWasSaved();
    }

    [Theory]
    [InlineData(ProjectStatus.Cancelled)]
    [InlineData(ProjectStatus.Archived)]
    public async Task Handle_ProjectDoesNotAcceptTasks_ReturnsConflictAndCreatesNothing(ProjectStatus projectStatus)
    {
        // Arrange
        var project = TestData.CreateProject(projectStatus);
        ProjectExists(project);
        SignInAs(TestData.OwnerId);
        AuthorizationSucceeds();

        // Act
        var result = await CreateHandler().Handle(CommandFor(project.Id), CancellationToken.None);

        // Assert
        AssertFailure(result, "Project.NotAcceptingTasks", ErrorType.Conflict);
        NoTaskWasCreated();
        VerifyNothingWasSaved();
    }

    [Fact]
    public async Task Handle_UserNotAuthenticated_ReturnsUnauthorizedAndCreatesNothing()
    {
        // Arrange: no SignInAs(...) => nobody is logged in
        var project = TestData.CreateProject();
        ProjectExists(project);

        // Act
        var result = await CreateHandler().Handle(CommandFor(project.Id), CancellationToken.None);

        // Assert
        AssertFailure(result, "User.NotAuthenticated", ErrorType.Unauthorized);
        NoTaskWasCreated();
        VerifyNothingWasSaved();
    }

    [Fact]
    public async Task Handle_UserIsNotTheProjectOwner_ReturnsForbiddenAndCreatesNothing()
    {
        // Arrange
        var project = TestData.CreateProject(createdByUserId: TestData.OwnerId);
        ProjectExists(project);
        SignInAs(TestData.OtherUserId);
        AuthorizationFails();

        // Act
        var result = await CreateHandler().Handle(CommandFor(project.Id), CancellationToken.None);

        // Assert
        AssertFailure(result, "Project.Forbidden", ErrorType.Forbidden);
        NoTaskWasCreated();
        VerifyNothingWasSaved();
    }
}
