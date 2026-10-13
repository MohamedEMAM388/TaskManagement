using System.Security.Claims;
using Application.Common.Identity;
using Application.Common.ResultPattern;
using Application.Contracts;
using Microsoft.AspNetCore.Authorization;
using Moq;

namespace TaskManagement.UnitTests.Common;

/// <summary>
/// Shared setup for handler tests.
/// Only the EXTERNAL collaborators are mocked (database access, current user, authorization).
/// The handler itself and the domain entities are always the real code.
/// xUnit creates a new instance of the test class for every test, so these mocks start clean each time.
/// </summary>
public abstract class HandlerTestBase
{
    protected readonly Mock<IUnitOfWork> UnitOfWork = new();
    protected readonly Mock<IProjectRepository> ProjectRepo = new();
    protected readonly Mock<ITaskRepository> TaskRepo = new();
    protected readonly Mock<ICommentRepository> CommentRepo = new();
    protected readonly Mock<IUserService> UserService = new();
    protected readonly Mock<IAuthorizationService> Authorization = new();

    protected HandlerTestBase()
    {
        UnitOfWork.Setup(u => u.ProjectRepository).Returns(ProjectRepo.Object);
        UnitOfWork.Setup(u => u.TaskRepository).Returns(TaskRepo.Object);
        UnitOfWork.Setup(u => u.CommentRepository).Returns(CommentRepo.Object);
        UnitOfWork.Setup(u => u.SaveChangesAsync(It.IsAny<CancellationToken>())).ReturnsAsync(1);
    }

    // Not calling this = nobody is logged in (UserService returns null).
    protected void SignInAs(string userId)
    {
        UserService.Setup(u => u.UserId).Returns(userId);
        UserService.Setup(u => u.User).Returns(TestData.CreateUser(userId));
    }

    protected void AuthorizationSucceeds() =>
        Authorization
            .Setup(a => a.AuthorizeAsync(It.IsAny<ClaimsPrincipal>(), It.IsAny<object>(), "ResourceOwner"))
            .ReturnsAsync(AuthorizationResult.Success());

    protected void AuthorizationFails() =>
        Authorization
            .Setup(a => a.AuthorizeAsync(It.IsAny<ClaimsPrincipal>(), It.IsAny<object>(), "ResourceOwner"))
            .ReturnsAsync(AuthorizationResult.Failed());

    protected void VerifyNothingWasSaved() =>
        UnitOfWork.Verify(u => u.SaveChangesAsync(It.IsAny<CancellationToken>()), Times.Never());

    protected static void AssertFailure(Result result, string expectedCode, ErrorType expectedType)
    {
        Assert.False(result.IsSuccess);
        var error = Assert.Single(result.Errors);
        Assert.Equal(expectedCode, error.Code);
        Assert.Equal(expectedType, error.ErrorType);
    }
}
