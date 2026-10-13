using TaskManagement.UnitTests.Common;

namespace TaskManagement.UnitTests.Tasks;

// Pure domain rule: no mocks needed.
public class TaskAssignmentTests
{
    [Fact]
    public void NewTask_IsUnassigned()
    {
        var task = TestData.CreateTask();

        Assert.Null(task.AssigneeId);
    }

    [Fact]
    public void AssignTo_ValidUserId_SetsAssignee()
    {
        // Arrange
        var task = TestData.CreateTask();

        // Act
        task.AssignTo("user-123");

        // Assert
        Assert.Equal("user-123", task.AssigneeId);
    }

    [Fact]
    public void AssignTo_AlreadyAssignedTask_ReplacesAssignee()
    {
        var task = TestData.CreateTask();
        task.AssignTo("first-user");

        task.AssignTo("second-user");

        Assert.Equal("second-user", task.AssigneeId);
    }

    [Theory]
    [InlineData("")]
    [InlineData("   ")]
    public void AssignTo_BlankUserId_ThrowsAndKeepsAssignee(string blankUserId)
    {
        // Arrange
        var task = TestData.CreateTask();
        task.AssignTo("first-user");

        // Act
        Assert.ThrowsAny<ArgumentException>(() => task.AssignTo(blankUserId));

        // Assert
        Assert.Equal("first-user", task.AssigneeId);
    }
}
