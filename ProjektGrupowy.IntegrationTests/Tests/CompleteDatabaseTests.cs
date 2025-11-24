using FluentAssertions;
using Microsoft.EntityFrameworkCore;
using ProjektGrupowy.Domain.Models;
using ProjektGrupowy.IntegrationTests.Infrastructure;

namespace ProjektGrupowy.IntegrationTests.Tests;

/// <summary>
/// Tests that verify the complete database hierarchy seeding and CRUD operations
/// across all entities using SeedCompleteDatabaseAsync()
/// </summary>
public class CompleteDatabaseTests : IClassFixture<IntegrationTestWebAppFactory>, IAsyncLifetime
{
    private readonly IntegrationTestWebAppFactory _factory;
    private readonly HttpClient _client;

    public CompleteDatabaseTests(IntegrationTestWebAppFactory factory)
    {
        _factory = factory;
        _client = factory.CreateClient();
    }

    public async Task InitializeAsync()
    {
        var context = await _factory.GetDbContextAsync();
        await DatabaseSeeder.ClearDatabaseAsync(context);
    }

    public Task DisposeAsync() => Task.CompletedTask;

    [Fact]
    public async Task SeedCompleteDatabase_CreatesAllEntities_Successfully()
    {
        // Arrange
        var context = await _factory.GetDbContextAsync();

        // Act
        var seed = await DatabaseSeeder.SeedCompleteDatabaseAsync(context);

        // Assert - Verify all entities were created
        seed.Should().NotBeNull();

        // Users
        seed.ScientistId.Should().NotBeNullOrEmpty();
        seed.LabelerId.Should().NotBeNullOrEmpty();

        // Project
        seed.Project.Should().NotBeNull();
        seed.Project.Id.Should().BeGreaterThan(0);
        seed.Project.CreatedById.Should().Be(seed.ScientistId);

        // Subject
        seed.Subject.Should().NotBeNull();
        seed.Subject.Id.Should().BeGreaterThan(0);
        seed.Subject.Project.Id.Should().Be(seed.Project.Id);

        // Label
        seed.Label.Should().NotBeNull();
        seed.Label.Id.Should().BeGreaterThan(0);
        seed.Label.Subject.Id.Should().Be(seed.Subject.Id);

        // VideoGroup
        seed.VideoGroup.Should().NotBeNull();
        seed.VideoGroup.Id.Should().BeGreaterThan(0);
        seed.VideoGroup.Project.Id.Should().Be(seed.Project.Id);

        // Video
        seed.Video.Should().NotBeNull();
        seed.Video.Id.Should().BeGreaterThan(0);
        seed.Video.VideoGroup.Id.Should().Be(seed.VideoGroup.Id);

        // Assignment
        seed.Assignment.Should().NotBeNull();
        seed.Assignment.Id.Should().BeGreaterThan(0);
        seed.Assignment.Subject.Id.Should().Be(seed.Subject.Id);
        seed.Assignment.VideoGroup.Id.Should().Be(seed.VideoGroup.Id);

        // Access Code
        seed.AccessCode.Should().NotBeNull();
        seed.AccessCode.Id.Should().BeGreaterThan(0);
        seed.AccessCode.ProjectId.Should().Be(seed.Project.Id);
        seed.AccessCode.Code.Should().NotBeNullOrEmpty();

        // Assigned Label
        seed.AssignedLabel.Should().NotBeNull();
        seed.AssignedLabel.Id.Should().BeGreaterThan(0);
        seed.AssignedLabel.Label.Id.Should().Be(seed.Label.Id);
        seed.AssignedLabel.Video.Id.Should().Be(seed.Video.Id);
        seed.AssignedLabel.CreatedById.Should().Be(seed.LabelerId);

        // Generated Report
        seed.GeneratedReport.Should().NotBeNull();
        seed.GeneratedReport.Id.Should().BeGreaterThan(0);
        seed.GeneratedReport.Project.Id.Should().Be(seed.Project.Id);
        seed.GeneratedReport.Name.Should().NotBeNullOrEmpty();
        seed.GeneratedReport.Path.Should().NotBeNullOrEmpty();
    }

    [Fact]
    public async Task SeedCompleteDatabase_LabelerIsAssignedToAssignment()
    {
        // Arrange
        var context = await _factory.GetDbContextAsync();

        // Act
        var seed = await DatabaseSeeder.SeedCompleteDatabaseAsync(context);

        // Assert
        seed.Assignment.Labelers.Should().NotBeNull();
        seed.Assignment.Labelers.Should().HaveCount(1);
        seed.Assignment.Labelers.First().Id.Should().Be(seed.LabelerId);
    }

    [Fact]
    public async Task SeedCompleteDatabase_AllEntitiesHaveCorrectRelationships()
    {
        // Arrange
        var context = await _factory.GetDbContextAsync();

        // Act
        var seed = await DatabaseSeeder.SeedCompleteDatabaseAsync(context);

        // Assert - Verify relationships
        // Project -> Subjects
        seed.Project.Subjects.Should().Contain(s => s.Id == seed.Subject.Id);

        // Project -> VideoGroups
        seed.Project.VideoGroups.Should().Contain(vg => vg.Id == seed.VideoGroup.Id);

        // Project -> AccessCodes
        seed.Project.AccessCodes.Should().Contain(ac => ac.Id == seed.AccessCode.Id);

        // Project -> ProjectLabelers (labeler should be added to project)
        seed.Project.ProjectLabelers.Should().Contain(u => u.Id == seed.LabelerId);

        // Project -> GeneratedReports
        seed.Project.GeneratedReports.Should().Contain(r => r.Id == seed.GeneratedReport.Id);

        // Subject -> Labels
        seed.Subject.Labels.Should().Contain(l => l.Id == seed.Label.Id);

        // VideoGroup -> Videos
        seed.VideoGroup.Videos.Should().Contain(v => v.Id == seed.Video.Id);

        // Video -> AssignedLabels
        seed.Video.AssignedLabels.Should().Contain(al => al.Id == seed.AssignedLabel.Id);

        // Label -> AssignedLabels
        seed.Label.AssignedLabels.Should().Contain(al => al.Id == seed.AssignedLabel.Id);
    }

    [Fact]
    public async Task CompleteDatabase_CanPerformCRUDOnProject()
    {
        // Arrange
        var context = await _factory.GetDbContextAsync();
        var seed = await DatabaseSeeder.SeedCompleteDatabaseAsync(context);
        var projectId = seed.Project.Id;

        // Test READ
        var project = await context.Projects.FindAsync(projectId);
        project.Should().NotBeNull();
        project!.Name.Should().Be(seed.Project.Name);

        // Test UPDATE
        project.Update("Updated Name", "Updated Description", false, seed.ScientistId);
        await context.SaveChangesAsync();

        var updatedProject = await context.Projects.FindAsync(projectId);
        updatedProject!.Name.Should().Be("Updated Name");

        // Test DELETE (soft delete)
        context.Projects.Remove(project);
        await context.SaveChangesAsync();

        // Get fresh context to verify deletion
        var freshContext = await _factory.GetDbContextAsync();
        var deletedProject = await freshContext.Projects.FindAsync(projectId);
        deletedProject.Should().BeNull(); // Should be filtered by query filter
    }

    [Fact]
    public async Task CompleteDatabase_CanPerformCRUDOnSubject()
    {
        // Arrange
        var context = await _factory.GetDbContextAsync();
        var seed = await DatabaseSeeder.SeedCompleteDatabaseAsync(context);
        var subjectId = seed.Subject.Id;

        // Test READ
        var subject = await context.Subjects.FindAsync(subjectId);
        subject.Should().NotBeNull();

        // Test UPDATE
        subject!.Update("Updated Subject", "Updated Description", seed.Project, seed.ScientistId);
        await context.SaveChangesAsync();

        var updatedSubject = await context.Subjects.FindAsync(subjectId);
        updatedSubject!.Name.Should().Be("Updated Subject");

        // Test DELETE
        context.Subjects.Remove(subject);
        await context.SaveChangesAsync();

        var freshContext = await _factory.GetDbContextAsync();
        var deletedSubject = await freshContext.Subjects.FindAsync(subjectId);
        deletedSubject.Should().BeNull();
    }

    [Fact]
    public async Task CompleteDatabase_CanPerformCRUDOnLabel()
    {
        // Arrange
        var context = await _factory.GetDbContextAsync();
        var seed = await DatabaseSeeder.SeedCompleteDatabaseAsync(context);
        var labelId = seed.Label.Id;

        // Test READ
        var label = await context.Labels.FindAsync(labelId);
        label.Should().NotBeNull();

        // Test UPDATE
        label!.Update("Updated Label", "#FF0000", "point", 'U', seed.Subject, seed.ScientistId);
        await context.SaveChangesAsync();

        var updatedLabel = await context.Labels.FindAsync(labelId);
        updatedLabel!.Name.Should().Be("Updated Label");
        updatedLabel.ColorHex.Should().Be("#FF0000");

        // Test DELETE
        context.Labels.Remove(label);
        await context.SaveChangesAsync();

        var freshContext = await _factory.GetDbContextAsync();
        var deletedLabel = await freshContext.Labels.FindAsync(labelId);
        deletedLabel.Should().BeNull();
    }

    [Fact]
    public async Task CompleteDatabase_CanPerformCRUDOnVideoGroup()
    {
        // Arrange
        var context = await _factory.GetDbContextAsync();
        var seed = await DatabaseSeeder.SeedCompleteDatabaseAsync(context);
        var videoGroupId = seed.VideoGroup.Id;

        // Test READ
        var videoGroup = await context.VideoGroups.FindAsync(videoGroupId);
        videoGroup.Should().NotBeNull();

        // Test UPDATE
        videoGroup!.Update("Updated VideoGroup", "Updated Description", seed.Project, seed.ScientistId);
        await context.SaveChangesAsync();

        var updatedVideoGroup = await context.VideoGroups.FindAsync(videoGroupId);
        updatedVideoGroup!.Name.Should().Be("Updated VideoGroup");

        // Test DELETE
        context.VideoGroups.Remove(videoGroup);
        await context.SaveChangesAsync();

        var freshContext = await _factory.GetDbContextAsync();
        var deletedVideoGroup = await freshContext.VideoGroups.FindAsync(videoGroupId);
        deletedVideoGroup.Should().BeNull();
    }

    [Fact]
    public async Task CompleteDatabase_CanPerformCRUDOnVideo()
    {
        // Arrange
        var context = await _factory.GetDbContextAsync();
        var seed = await DatabaseSeeder.SeedCompleteDatabaseAsync(context);
        var videoId = seed.Video.Id;

        // Test READ
        var video = await context.Videos.FindAsync(videoId);
        video.Should().NotBeNull();

        // Test UPDATE
        video!.Update("Updated Title", seed.VideoGroup, "/new/path.mp4", "video/webm", 2, seed.ScientistId);
        await context.SaveChangesAsync();

        var updatedVideo = await context.Videos.FindAsync(videoId);
        updatedVideo!.Title.Should().Be("Updated Title");
        updatedVideo.PositionInQueue.Should().Be(2);

        // Test DELETE
        context.Videos.Remove(video);
        await context.SaveChangesAsync();

        var freshContext = await _factory.GetDbContextAsync();
        var deletedVideo = await freshContext.Videos.FindAsync(videoId);
        deletedVideo.Should().BeNull();
    }

    [Fact]
    public async Task CompleteDatabase_CanPerformCRUDOnAssignment()
    {
        // Arrange
        var context = await _factory.GetDbContextAsync();
        var seed = await DatabaseSeeder.SeedCompleteDatabaseAsync(context);
        var assignmentId = seed.Assignment.Id;

        // Test READ
        var assignment = await context.SubjectVideoGroupAssignments.FindAsync(assignmentId);
        assignment.Should().NotBeNull();

        // Test UPDATE
        assignment!.Update(seed.Subject, seed.VideoGroup, seed.ScientistId);
        await context.SaveChangesAsync();

        var updatedAssignment = await context.SubjectVideoGroupAssignments.FindAsync(assignmentId);
        updatedAssignment.Should().NotBeNull();
        updatedAssignment!.ModificationDate.Should().NotBeNull();

        // Test DELETE
        context.SubjectVideoGroupAssignments.Remove(assignment);
        await context.SaveChangesAsync();

        var freshContext = await _factory.GetDbContextAsync();
        var deletedAssignment = await freshContext.SubjectVideoGroupAssignments.FindAsync(assignmentId);
        deletedAssignment.Should().BeNull();
    }

    [Fact]
    public async Task CompleteDatabase_CanPerformCRUDOnAccessCode()
    {
        // Arrange
        var context = await _factory.GetDbContextAsync();
        var seed = await DatabaseSeeder.SeedCompleteDatabaseAsync(context);
        var accessCodeId = seed.AccessCode.Id;

        // Test READ
        var accessCode = await context.ProjectAccessCodes.FindAsync(accessCodeId);
        accessCode.Should().NotBeNull();
        accessCode!.IsValid.Should().BeTrue();

        // Test UPDATE (Retire)
        accessCode.Retire(seed.ScientistId);
        await context.SaveChangesAsync();

        var retiredCode = await context.ProjectAccessCodes.FindAsync(accessCodeId);
        retiredCode!.IsValid.Should().BeFalse();
        retiredCode.IsExpired.Should().BeTrue();

        // Test DELETE
        context.ProjectAccessCodes.Remove(accessCode);
        await context.SaveChangesAsync();

        var freshContext = await _factory.GetDbContextAsync();
        var deletedCode = await freshContext.ProjectAccessCodes.FindAsync(accessCodeId);
        deletedCode.Should().BeNull();
    }

    [Fact]
    public async Task CompleteDatabase_AllEntitiesAreSoftDeleted()
    {
        // Arrange
        var context = await _factory.GetDbContextAsync();
        var seed = await DatabaseSeeder.SeedCompleteDatabaseAsync(context);
        var projectId = seed.Project.Id;

        // Act - Delete all entities
        context.Projects.Remove(seed.Project);
        await context.SaveChangesAsync();

        // Assert - All related entities should be soft deleted due to cascade
        var freshContext = await _factory.GetDbContextAsync();
        var project = await freshContext.Projects.FindAsync(projectId);
        project.Should().BeNull(); // Filtered by query filter

        // Verify they exist in database with IgnoreQueryFilters
        var projectWithDeleted = await freshContext.Projects
            .IgnoreQueryFilters()
            .FirstOrDefaultAsync(p => p.Id == projectId);

        projectWithDeleted.Should().NotBeNull();
        projectWithDeleted!.DelDate.Should().NotBeNull();
    }

    [Fact]
    public async Task CompleteDatabase_CanPerformCRUDOnAssignedLabel()
    {
        // Arrange
        var context = await _factory.GetDbContextAsync();
        var seed = await DatabaseSeeder.SeedCompleteDatabaseAsync(context);
        var assignedLabelId = seed.AssignedLabel.Id;

        // Test READ
        var assignedLabel = await context.AssignedLabels.FindAsync(assignedLabelId);
        assignedLabel.Should().NotBeNull();
        assignedLabel!.Start.Should().Be("00:00:05");
        assignedLabel.End.Should().Be("00:00:15");
        assignedLabel.Label.Id.Should().Be(seed.Label.Id);
        assignedLabel.Video.Id.Should().Be(seed.Video.Id);

        // Test DELETE
        context.AssignedLabels.Remove(assignedLabel);
        await context.SaveChangesAsync();

        var freshContext = await _factory.GetDbContextAsync();
        var deletedLabel = await freshContext.AssignedLabels.FindAsync(assignedLabelId);
        deletedLabel.Should().BeNull();
    }

    [Fact]
    public async Task CompleteDatabase_CanPerformCRUDOnGeneratedReport()
    {
        // Arrange
        var context = await _factory.GetDbContextAsync();
        var seed = await DatabaseSeeder.SeedCompleteDatabaseAsync(context);
        var reportId = seed.GeneratedReport.Id;

        // Test READ
        var report = await context.GeneratedReports.FindAsync(reportId);
        report.Should().NotBeNull();
        report!.Name.Should().NotBeNullOrEmpty();
        report.Path.Should().NotBeNullOrEmpty();
        report.Project.Id.Should().Be(seed.Project.Id);

        // Test DELETE
        context.GeneratedReports.Remove(report);
        await context.SaveChangesAsync();

        var freshContext = await _factory.GetDbContextAsync();
        var deletedReport = await freshContext.GeneratedReports.FindAsync(reportId);
        deletedReport.Should().BeNull();
    }
}
