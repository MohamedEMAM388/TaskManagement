using System.Security.Claims;
using Domain.Entities.Enums;
using Project = Domain.Entities.Project;
using DomainTask = Domain.Entities.Task;
using TaskStatus = Domain.Entities.Enums.TaskStatus;

namespace TaskManagement.UnitTests.Common;

/// <summary>
/// Small factory methods so every test builds its data in one readable line.
/// </summary>
internal static class TestData
{
    public const string OwnerId = "owner-user-id";
    public const string OtherUserId = "other-user-id";

    public static DomainTask CreateTask(
        TaskStatus status = TaskStatus.Todo,
        int id = 1,
        int projectId = 10,
        string createdByUserId = OwnerId) => new()
    {
        Id = id,
        Title = "Write report",
        Description = "Quarterly report",
        Status = status,
        DueDate = new DateTime(2030, 1, 1),
        ProjectId = projectId,
        CreatedByUserId = createdByUserId
    };

    public static Project CreateProject(
        ProjectStatus status = ProjectStatus.InProgress,
        int id = 10,
        string createdByUserId = OwnerId) => new()
    {
        Id = id,
        Name = "Website Redesign",
        Description = "Redesign the company website",
        Status = status,
        CreatedByUserId = createdByUserId
    };

    public static ClaimsPrincipal CreateUser(string userId, params string[] roles)
    {
        var claims = new List<Claim> { new(ClaimTypes.NameIdentifier, userId) };
        claims.AddRange(roles.Select(role => new Claim(ClaimTypes.Role, role)));

        // The authenticationType ("TestAuth") makes the identity count as authenticated.
        return new ClaimsPrincipal(new ClaimsIdentity(claims, "TestAuth"));
    }
}
