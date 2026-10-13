using System.Security.Claims;
using Application.Common.Authorization;
using Domain.Common;
using Microsoft.AspNetCore.Authorization;
using TaskManagement.UnitTests.Common;

namespace TaskManagement.UnitTests.Authorization;

// The "only the owner (or an Admin) can modify a resource" rule. Real handler, no mocks.
public class ResourceOwnerAuthorizationHandlerTests
{
    private static async Task<bool> IsAuthorizedAsync(ClaimsPrincipal user, IHasOwner resource)
    {
        var requirement = new ResourceOwnerRequirement();
        var context = new AuthorizationHandlerContext(new[] { requirement }, user, resource);

        await new ResourceOwnerAuthorizationHandler().HandleAsync(context);

        return context.HasSucceeded;
    }

    [Fact]
    public async Task Owner_IsAuthorized()
    {
        var task = TestData.CreateTask(createdByUserId: TestData.OwnerId);
        var owner = TestData.CreateUser(TestData.OwnerId, "User");

        Assert.True(await IsAuthorizedAsync(owner, task));
    }

    [Fact]
    public async Task Admin_IsAuthorizedEvenIfNotTheOwner()
    {
        var task = TestData.CreateTask(createdByUserId: TestData.OwnerId);
        var admin = TestData.CreateUser(TestData.OtherUserId, "Admin");

        Assert.True(await IsAuthorizedAsync(admin, task));
    }

    [Fact]
    public async Task AnotherUser_IsNotAuthorized()
    {
        var task = TestData.CreateTask(createdByUserId: TestData.OwnerId);
        var stranger = TestData.CreateUser(TestData.OtherUserId, "User");

        Assert.False(await IsAuthorizedAsync(stranger, task));
    }

    [Fact]
    public async Task AnonymousUser_IsNotAuthorized()
    {
        var task = TestData.CreateTask(createdByUserId: TestData.OwnerId);
        var anonymous = new ClaimsPrincipal(new ClaimsIdentity()); // not authenticated, no claims

        Assert.False(await IsAuthorizedAsync(anonymous, task));
    }
}
