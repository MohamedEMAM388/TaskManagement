using Domain.Exceptions;
using TaskManagement.UnitTests.Common;
using TaskStatus = Domain.Entities.Enums.TaskStatus;

namespace TaskManagement.UnitTests.Tasks;

// Pure domain rules: no mocks needed, because Task has no external collaborators.
public class TaskStatusTransitionTests
{
    // ---------- valid transitions ----------

    [Theory]
    [InlineData(TaskStatus.Todo, TaskStatus.InProgress)]
    [InlineData(TaskStatus.Todo, TaskStatus.Cancelled)]
    [InlineData(TaskStatus.InProgress, TaskStatus.Completed)]
    [InlineData(TaskStatus.InProgress, TaskStatus.Cancelled)]
    public void ChangeStatus_ValidTransition_UpdatesStatus(TaskStatus from, TaskStatus to)
    {
        // Arrange
        var task = TestData.CreateTask(from);

        // Act
        task.ChangeStatus(to);

        // Assert
        Assert.Equal(to, task.Status);
    }

    [Theory]
    [InlineData(TaskStatus.Todo, TaskStatus.InProgress)]
    [InlineData(TaskStatus.InProgress, TaskStatus.Completed)]
    public void CanChangeStatusTo_ValidTransition_ReturnsTrue(TaskStatus from, TaskStatus to)
    {
        var task = TestData.CreateTask(from);

        var allowed = task.CanChangeStatusTo(to);

        Assert.True(allowed);
    }

    // ---------- invalid transitions ----------

    [Theory]
    [InlineData(TaskStatus.Todo, TaskStatus.Completed)]        // can't skip InProgress
    [InlineData(TaskStatus.Todo, TaskStatus.Todo)]             // same status
    [InlineData(TaskStatus.InProgress, TaskStatus.Todo)]       // can't go backwards
    [InlineData(TaskStatus.Completed, TaskStatus.Todo)]        // Completed is final
    [InlineData(TaskStatus.Completed, TaskStatus.InProgress)]
    [InlineData(TaskStatus.Completed, TaskStatus.Cancelled)]
    [InlineData(TaskStatus.Cancelled, TaskStatus.Todo)]        // Cancelled is final
    [InlineData(TaskStatus.Cancelled, TaskStatus.InProgress)]
    [InlineData(TaskStatus.Cancelled, TaskStatus.Completed)]
    public void ChangeStatus_InvalidTransition_ThrowsAndKeepsOldStatus(TaskStatus from, TaskStatus to)
    {
        // Arrange
        var task = TestData.CreateTask(from);

        // Act
        var exception = Assert.Throws<InvalidTaskStatusTransitionException>(() => task.ChangeStatus(to));

        // Assert
        Assert.Equal(from, task.Status);
        Assert.Contains(from.ToString(), exception.Message);
        Assert.Contains(to.ToString(), exception.Message);
    }

    [Theory]
    [InlineData(TaskStatus.Todo, TaskStatus.Completed)]
    [InlineData(TaskStatus.Completed, TaskStatus.Todo)]
    [InlineData(TaskStatus.Cancelled, TaskStatus.InProgress)]
    public void CanChangeStatusTo_InvalidTransition_ReturnsFalse(TaskStatus from, TaskStatus to)
    {
        var task = TestData.CreateTask(from);

        var allowed = task.CanChangeStatusTo(to);

        Assert.False(allowed);
    }

    // ---------- closed tasks ----------

    [Theory]
    [InlineData(TaskStatus.Completed, true)]
    [InlineData(TaskStatus.Cancelled, true)]
    [InlineData(TaskStatus.Todo, false)]
    [InlineData(TaskStatus.InProgress, false)]
    public void IsClosed_ReflectsWhetherTheTaskIsInAFinalStatus(TaskStatus status, bool expectedClosed)
    {
        var task = TestData.CreateTask(status);

        Assert.Equal(expectedClosed, task.IsClosed);
    }
}
