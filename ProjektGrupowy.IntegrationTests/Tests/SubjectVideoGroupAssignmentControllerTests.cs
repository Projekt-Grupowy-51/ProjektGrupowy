using System.Net;
using System.Net.Http.Json;
using FluentAssertions;
using Microsoft.EntityFrameworkCore;
using ProjektGrupowy.API.DTOs.Labeler;
using ProjektGrupowy.API.DTOs.SubjectVideoGroupAssignment;
using ProjektGrupowy.IntegrationTests.Helpers;
using ProjektGrupowy.IntegrationTests.Infrastructure;

namespace ProjektGrupowy.IntegrationTests.Tests;

public class SubjectVideoGroupAssignmentControllerTests : IClassFixture<IntegrationTestWebAppFactory>, IAsyncLifetime
{
    private readonly IntegrationTestWebAppFactory _factory;
    private readonly HttpClient _client;

    public SubjectVideoGroupAssignmentControllerTests(IntegrationTestWebAppFactory factory)
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
    public async Task GetSubjectVideoGroupAssignments_AsScientist_ReturnsOwnAssignments()
    {
        // Arrange
        var context = await _factory.GetDbContextAsync();
        var scientistId = Guid.NewGuid().ToString();
        var assignment1 = await DatabaseSeeder.SeedSubjectVideoGroupAssignmentAsync(context, userId: scientistId);
        var assignment2 = await DatabaseSeeder.SeedSubjectVideoGroupAssignmentAsync(context, userId: scientistId);

        // Act
        var client = _client.WithScientistUser(scientistId);
        var response = await client.GetAsync("/api/subject-video-group-assignments");

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.OK);
        var assignments = await response.Content.ReadFromJsonAsync<List<SubjectVideoGroupAssignmentResponse>>();
        assignments.Should().NotBeNull();
        assignments!.Should().HaveCountGreaterOrEqualTo(2);
        assignments.Should().Contain(a => a.Id == assignment1.Id);
        assignments.Should().Contain(a => a.Id == assignment2.Id);
    }

    [Fact]
    public async Task GetSubjectVideoGroupAssignments_AsAdmin_ReturnsAllAssignments()
    {
        // Arrange
        var context = await _factory.GetDbContextAsync();
        var scientistId = Guid.NewGuid().ToString();
        var assignment = await DatabaseSeeder.SeedSubjectVideoGroupAssignmentAsync(context, userId: scientistId);

        // Act
        var adminId = Guid.NewGuid().ToString();
        var client = _client.WithAdminUser(adminId);
        var response = await client.GetAsync("/api/subject-video-group-assignments");

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.OK);
        var assignments = await response.Content.ReadFromJsonAsync<List<SubjectVideoGroupAssignmentResponse>>();
        assignments.Should().NotBeNull();
        assignments!.Should().HaveCountGreaterOrEqualTo(1);
        assignments.Should().Contain(a => a.Id == assignment.Id);
    }

    [Fact]
    public async Task GetSubjectVideoGroupAssignment_AsScientist_ReturnsOwnAssignment()
    {
        // Arrange
        var context = await _factory.GetDbContextAsync();
        var scientistId = Guid.NewGuid().ToString();
        var assignment = await DatabaseSeeder.SeedSubjectVideoGroupAssignmentAsync(context, userId: scientistId);

        // Act
        var client = _client.WithScientistUser(scientistId);
        var response = await client.GetAsync($"/api/subject-video-group-assignments/{assignment.Id}");

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.OK);
        var assignmentResponse = await response.Content.ReadFromJsonAsync<SubjectVideoGroupAssignmentResponse>();
        assignmentResponse.Should().NotBeNull();
        assignmentResponse!.Id.Should().Be(assignment.Id);
        assignmentResponse.SubjectId.Should().Be(assignment.Subject.Id);
        assignmentResponse.VideoGroupId.Should().Be(assignment.VideoGroup.Id);
    }

    [Fact]
    public async Task GetSubjectVideoGroupAssignment_ForNonExistentAssignment_ReturnsNotFound()
    {
        // Arrange
        var scientistId = Guid.NewGuid().ToString();
        var nonExistentAssignmentId = 99999;

        // Act
        var client = _client.WithScientistUser(scientistId);
        var response = await client.GetAsync($"/api/subject-video-group-assignments/{nonExistentAssignmentId}");

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.NotFound);
    }

    [Fact]
    public async Task GetSubjectVideoGroupAssignmentLabelers_AsScientist_ReturnsLabelers()
    {
        // Arrange
        var context = await _factory.GetDbContextAsync();
        var scientistId = Guid.NewGuid().ToString();
        var assignment = await DatabaseSeeder.SeedSubjectVideoGroupAssignmentAsync(context, userId: scientistId);

        // Act
        var client = _client.WithScientistUser(scientistId);
        var response = await client.GetAsync($"/api/subject-video-group-assignments/{assignment.Id}/labelers");

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.OK);
        var labelers = await response.Content.ReadFromJsonAsync<List<LabelerResponse>>();
        labelers.Should().NotBeNull();
    }

    [Fact]
    public async Task AddSubjectVideoGroupAssignment_AsScientist_ReturnsCreated()
    {
        // Arrange
        var context = await _factory.GetDbContextAsync();
        var scientistId = Guid.NewGuid().ToString();
        var subject = await DatabaseSeeder.SeedSubjectAsync(context, null, scientistId);
        var videoGroup = await DatabaseSeeder.SeedVideoGroupAsync(context, subject.Project.Id, scientistId);

        var assignmentRequest = new SubjectVideoGroupAssignmentRequest
        {
            SubjectId = subject.Id,
            VideoGroupId = videoGroup.Id
        };

        // Act
        var client = _client.WithScientistUser(scientistId);
        var response = await client.PostAsJsonAsync("/api/subject-video-group-assignments", assignmentRequest);

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.Created);
        var createdAssignment = await response.Content.ReadFromJsonAsync<SubjectVideoGroupAssignmentResponse>();
        createdAssignment.Should().NotBeNull();
        createdAssignment!.SubjectId.Should().Be(subject.Id);
        createdAssignment.VideoGroupId.Should().Be(videoGroup.Id);
    }

    [Fact]
    public async Task AddSubjectVideoGroupAssignment_AsLabeler_ReturnsForbidden()
    {
        // Arrange
        var context = await _factory.GetDbContextAsync();
        var scientistId = Guid.NewGuid().ToString();
        var subject = await DatabaseSeeder.SeedSubjectAsync(context, null, scientistId);
        var videoGroup = await DatabaseSeeder.SeedVideoGroupAsync(context, subject.Project.Id, scientistId);

        var assignmentRequest = new SubjectVideoGroupAssignmentRequest
        {
            SubjectId = subject.Id,
            VideoGroupId = videoGroup.Id
        };

        // Act
        var labelerId = Guid.NewGuid().ToString();
        var client = _client.WithLabelerUser(labelerId);
        var response = await client.PostAsJsonAsync("/api/subject-video-group-assignments", assignmentRequest);

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.Forbidden);
    }

    [Fact]
    public async Task UpdateSubjectVideoGroupAssignment_AsScientist_ReturnsNoContent()
    {
        // Arrange
        var context = await _factory.GetDbContextAsync();
        var scientistId = Guid.NewGuid().ToString();
        var assignment = await DatabaseSeeder.SeedSubjectVideoGroupAssignmentAsync(context, userId: scientistId);
        var newSubject = await DatabaseSeeder.SeedSubjectAsync(context, null, scientistId);

        var updateRequest = new SubjectVideoGroupAssignmentRequest
        {
            SubjectId = newSubject.Id,
            VideoGroupId = assignment.VideoGroup.Id
        };

        // Act
        var client = _client.WithScientistUser(scientistId);
        var response = await client.PutAsJsonAsync($"/api/subject-video-group-assignments/{assignment.Id}", updateRequest);

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.NoContent);

        // Verify update
        var updatedContext = await _factory.GetDbContextAsync();
        var updatedAssignment = await updatedContext.SubjectVideoGroupAssignments
            .Include(a => a.Subject)
            .FirstOrDefaultAsync(a => a.Id == assignment.Id);
        updatedAssignment.Should().NotBeNull();
        updatedAssignment!.Subject.Id.Should().Be(newSubject.Id);
    }

    [Fact]
    public async Task DeleteSubjectVideoGroupAssignment_AsScientist_ReturnsNoContent()
    {
        // Arrange
        var context = await _factory.GetDbContextAsync();
        var scientistId = Guid.NewGuid().ToString();
        var assignment = await DatabaseSeeder.SeedSubjectVideoGroupAssignmentAsync(context, userId: scientistId);

        // Act
        var client = _client.WithScientistUser(scientistId);
        var response = await client.DeleteAsync($"/api/subject-video-group-assignments/{assignment.Id}");

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.NoContent);

        // Verify deletion
        var getResponse = await client.GetAsync($"/api/subject-video-group-assignments/{assignment.Id}");
        getResponse.StatusCode.Should().Be(HttpStatusCode.NotFound);
    }

    [Fact]
    public async Task DeleteSubjectVideoGroupAssignment_ForNonExistentAssignment_ReturnsNotFound()
    {
        // Arrange
        var scientistId = Guid.NewGuid().ToString();
        var nonExistentAssignmentId = 99999;

        // Act
        var client = _client.WithScientistUser(scientistId);
        var response = await client.DeleteAsync($"/api/subject-video-group-assignments/{nonExistentAssignmentId}");

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.NotFound);
    }

    [Fact]
    public async Task AssignLabelerToSubjectVideoGroup_AsScientist_ReturnsOk()
    {
        // Arrange
        var context = await _factory.GetDbContextAsync();
        var scientistId = Guid.NewGuid().ToString();
        var assignment = await DatabaseSeeder.SeedSubjectVideoGroupAssignmentAsync(context, userId: scientistId);
        var labelerId = Guid.NewGuid().ToString();

        // Ensure labeler exists in database
        await DatabaseSeeder.EnsureUserExistsAsync(context, labelerId);

        // Act
        var client = _client.WithScientistUser(scientistId);
        var response = await client.PostAsync($"/api/subject-video-group-assignments/{assignment.Id}/assign-labeler/{labelerId}", null);

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.OK);

        // Verify labeler was assigned
        var updatedContext = await _factory.GetDbContextAsync();
        var updatedAssignment = await updatedContext.SubjectVideoGroupAssignments
            .Include(a => a.Labelers)
            .FirstOrDefaultAsync(a => a.Id == assignment.Id);
        updatedAssignment.Should().NotBeNull();
        updatedAssignment!.Labelers.Should().Contain(l => l.Id == labelerId);
    }

    [Fact]
    public async Task UnassignLabelerFromSubjectVideoGroup_AsScientist_ReturnsOk()
    {
        // Arrange
        var context = await _factory.GetDbContextAsync();
        var scientistId = Guid.NewGuid().ToString();
        var assignment = await DatabaseSeeder.SeedSubjectVideoGroupAssignmentAsync(context, userId: scientistId);
        var labelerId = Guid.NewGuid().ToString();

        // Ensure labeler exists in database
        await DatabaseSeeder.EnsureUserExistsAsync(context, labelerId);

        // First assign the labeler
        var client = _client.WithScientistUser(scientistId);
        await client.PostAsync($"/api/subject-video-group-assignments/{assignment.Id}/assign-labeler/{labelerId}", null);

        // Act - Unassign the labeler
        var response = await client.DeleteAsync($"/api/subject-video-group-assignments/{assignment.Id}/unassign-labeler/{labelerId}");

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.OK);

        // Verify labeler was unassigned
        var updatedContext = await _factory.GetDbContextAsync();
        var updatedAssignment = await updatedContext.SubjectVideoGroupAssignments
            .Include(a => a.Labelers)
            .FirstOrDefaultAsync(a => a.Id == assignment.Id);
        updatedAssignment.Should().NotBeNull();
        updatedAssignment!.Labelers.Should().NotContain(l => l.Id == labelerId);
    }
}
