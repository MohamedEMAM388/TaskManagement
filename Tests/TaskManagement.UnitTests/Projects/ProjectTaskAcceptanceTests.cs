using Domain.Entities.Enums;
using TaskManagement.UnitTests.Common;

namespace TaskManagement.UnitTests.Projects;

public class ProjectTaskAcceptanceTests
{
    [Theory]
    [InlineData(ProjectStatus.Pending, true)]
    [InlineData(ProjectStatus.Planning, true)]
    [InlineData(ProjectStatus.InProgress, true)]
    [InlineData(ProjectStatus.Completed, true)]
    [InlineData(ProjectStatus.Cancelled, false)]
    [InlineData(ProjectStatus.Archived, false)]
    public void CanAcceptTasks_DependsOnProjectStatus(ProjectStatus status, bool expected)
    {
        // Arrange
        var project = TestData.CreateProject(status);

        // Act
        var canAccept = project.CanAcceptTasks;

        // Assert
        Assert.Equal(expected, canAccept);
    }
}
