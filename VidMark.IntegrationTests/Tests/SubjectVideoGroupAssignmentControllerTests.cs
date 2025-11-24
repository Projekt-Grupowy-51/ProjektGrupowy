using System.Net;
using System.Net.Http.Json;
using FluentAssertions;
using Microsoft.EntityFrameworkCore;
using VidMark.API.DTOs.Labeler;
using VidMark.API.DTOs.SubjectVideoGroupAssignment;
using VidMark.IntegrationTests.Helpers;
using VidMark.IntegrationTests.Infrastructure;

namespace VidMark.IntegrationTests.Tests;

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
        var seed = await DatabaseSeeder.SeedCompleteDatabaseAsync(context);
        var client = _client.WithScientistUser(seed.ScientistId);

        // Act
        var response = await client.GetAsync("/api/subject-video-group-assignments");

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.OK);
        var assignments = await response.Content.ReadFromJsonAsync<List<SubjectVideoGroupAssignmentResponse>>();
        assignments.Should().NotBeNull().And.HaveCount(1);
        assignments![0].Id.Should().Be(seed.Assignment.Id);
        assignments[0].SubjectId.Should().Be(seed.Subject.Id);
        assignments[0].VideoGroupId.Should().Be(seed.VideoGroup.Id);
    }

    [Fact]
    public async Task GetSubjectVideoGroupAssignments_AsAdmin_ReturnsAllAssignments()
    {
        // Arrange
        var context = await _factory.GetDbContextAsync();
        var seed = await DatabaseSeeder.SeedCompleteDatabaseAsync(context);
        var adminId = Guid.NewGuid().ToString();
        var client = _client.WithAdminUser(adminId);

        // Act
        var response = await client.GetAsync("/api/subject-video-group-assignments");

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.OK);
        var assignments = await response.Content.ReadFromJsonAsync<List<SubjectVideoGroupAssignmentResponse>>();
        assignments.Should().NotBeNull().And.HaveCount(1);
        assignments![0].Id.Should().Be(seed.Assignment.Id);
    }

    [Fact]
    public async Task GetSubjectVideoGroupAssignment_AsScientist_ReturnsOwnAssignment()
    {
        // Arrange
        var context = await _factory.GetDbContextAsync();
        var seed = await DatabaseSeeder.SeedCompleteDatabaseAsync(context);
        var client = _client.WithScientistUser(seed.ScientistId);

        // Act
        var response = await client.GetAsync($"/api/subject-video-group-assignments/{seed.Assignment.Id}");

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.OK);
        var assignmentResponse = await response.Content.ReadFromJsonAsync<SubjectVideoGroupAssignmentResponse>();
        assignmentResponse.Should().NotBeNull();
        assignmentResponse!.Id.Should().Be(seed.Assignment.Id);
        assignmentResponse.SubjectId.Should().Be(seed.Subject.Id);
        assignmentResponse.VideoGroupId.Should().Be(seed.VideoGroup.Id);
    }

    [Fact]
    public async Task GetSubjectVideoGroupAssignment_WithInvalidId_ReturnsNotFound()
    {
        // Arrange
        var context = await _factory.GetDbContextAsync();
        var seed = await DatabaseSeeder.SeedCompleteDatabaseAsync(context);
        var client = _client.WithScientistUser(seed.ScientistId);

        // Act
        var response = await client.GetAsync("/api/subject-video-group-assignments/99999");

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.NotFound);
    }

    [Fact]
    public async Task GetSubjectVideoGroupAssignmentLabelers_AsScientist_ReturnsLabelers()
    {
        // Arrange
        var context = await _factory.GetDbContextAsync();
        var seed = await DatabaseSeeder.SeedCompleteDatabaseAsync(context);
        var client = _client.WithScientistUser(seed.ScientistId);

        // Act
        var response = await client.GetAsync($"/api/subject-video-group-assignments/{seed.Assignment.Id}/labelers");

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.OK);
        var labelers = await response.Content.ReadFromJsonAsync<List<LabelerResponse>>();
        labelers.Should().NotBeNull().And.HaveCount(1);
        labelers![0].Id.Should().Be(seed.LabelerId);
    }

    [Fact]
    public async Task AddSubjectVideoGroupAssignment_AsScientist_ReturnsCreated()
    {
        // Arrange
        var context = await _factory.GetDbContextAsync();
        var seed = await DatabaseSeeder.SeedCompleteDatabaseAsync(context);

        // Create another subject for the new assignment
        var newSubject = await DatabaseSeeder.SeedSubjectAsync(context, seed.Project.Id, seed.ScientistId);

        var assignmentRequest = new SubjectVideoGroupAssignmentRequest
        {
            SubjectId = newSubject.Id,
            VideoGroupId = seed.VideoGroup.Id
        };
        var client = _client.WithScientistUser(seed.ScientistId);

        // Act
        var response = await client.PostAsJsonAsync("/api/subject-video-group-assignments", assignmentRequest);

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.Created);
        response.Headers.Location.Should().NotBeNull();

        var createdAssignment = await response.Content.ReadFromJsonAsync<SubjectVideoGroupAssignmentResponse>();
        createdAssignment.Should().NotBeNull();
        createdAssignment!.SubjectId.Should().Be(newSubject.Id);
        createdAssignment.VideoGroupId.Should().Be(seed.VideoGroup.Id);
    }

    [Fact]
    public async Task AddSubjectVideoGroupAssignment_AsLabeler_ReturnsForbidden()
    {
        // Arrange
        var context = await _factory.GetDbContextAsync();
        var seed = await DatabaseSeeder.SeedCompleteDatabaseAsync(context);

        var assignmentRequest = new SubjectVideoGroupAssignmentRequest
        {
            SubjectId = seed.Subject.Id,
            VideoGroupId = seed.VideoGroup.Id
        };
        var client = _client.WithLabelerUser(seed.LabelerId);

        // Act
        var response = await client.PostAsJsonAsync("/api/subject-video-group-assignments", assignmentRequest);

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.Forbidden);
    }

    [Fact]
    public async Task UpdateSubjectVideoGroupAssignment_AsScientist_ReturnsNoContent()
    {
        // Arrange
        var context = await _factory.GetDbContextAsync();
        var seed = await DatabaseSeeder.SeedCompleteDatabaseAsync(context);

        // Create another subject for updating the assignment
        var newSubject = await DatabaseSeeder.SeedSubjectAsync(context, seed.Project.Id, seed.ScientistId);

        var updateRequest = new SubjectVideoGroupAssignmentRequest
        {
            SubjectId = newSubject.Id,
            VideoGroupId = seed.VideoGroup.Id
        };
        var client = _client.WithScientistUser(seed.ScientistId);

        // Act
        var response = await client.PutAsJsonAsync($"/api/subject-video-group-assignments/{seed.Assignment.Id}", updateRequest);

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.NoContent);

        // Verify update
        var updatedContext = await _factory.GetDbContextAsync();
        var updatedAssignment = await updatedContext.SubjectVideoGroupAssignments
            .Include(a => a.Subject)
            .FirstOrDefaultAsync(a => a.Id == seed.Assignment.Id);
        updatedAssignment.Should().NotBeNull();
        updatedAssignment!.Subject.Id.Should().Be(newSubject.Id);
    }

    [Fact]
    public async Task DeleteSubjectVideoGroupAssignment_AsScientist_ReturnsNoContent()
    {
        // Arrange
        var context = await _factory.GetDbContextAsync();
        var seed = await DatabaseSeeder.SeedCompleteDatabaseAsync(context);
        var client = _client.WithScientistUser(seed.ScientistId);

        // Act
        var response = await client.DeleteAsync($"/api/subject-video-group-assignments/{seed.Assignment.Id}");

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.NoContent);

        // Verify deletion (soft delete)
        var getResponse = await client.GetAsync($"/api/subject-video-group-assignments/{seed.Assignment.Id}");
        getResponse.StatusCode.Should().Be(HttpStatusCode.NotFound);
    }

    [Fact]
    public async Task DeleteSubjectVideoGroupAssignment_WithInvalidId_ReturnsNotFound()
    {
        // Arrange
        var context = await _factory.GetDbContextAsync();
        var seed = await DatabaseSeeder.SeedCompleteDatabaseAsync(context);
        var client = _client.WithScientistUser(seed.ScientistId);

        // Act
        var response = await client.DeleteAsync("/api/subject-video-group-assignments/99999");

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.NotFound);
    }

    [Fact]
    public async Task AssignLabelerToSubjectVideoGroup_AsScientist_ReturnsOk()
    {
        // Arrange
        var context = await _factory.GetDbContextAsync();
        var seed = await DatabaseSeeder.SeedCompleteDatabaseAsync(context);
        var newLabelerId = Guid.NewGuid().ToString();

        // Ensure new labeler exists in database
        await DatabaseSeeder.EnsureUserExistsAsync(context, newLabelerId);
        var client = _client.WithScientistUser(seed.ScientistId);

        // Act
        var response = await client.PostAsync($"/api/subject-video-group-assignments/{seed.Assignment.Id}/assign-labeler/{newLabelerId}", null);

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.OK);

        // Verify labeler was assigned
        var updatedContext = await _factory.GetDbContextAsync();
        var updatedAssignment = await updatedContext.SubjectVideoGroupAssignments
            .Include(a => a.Labelers)
            .FirstOrDefaultAsync(a => a.Id == seed.Assignment.Id);
        updatedAssignment.Should().NotBeNull();
        updatedAssignment!.Labelers.Should().Contain(l => l.Id == newLabelerId);
    }

    [Fact]
    public async Task UnassignLabelerFromSubjectVideoGroup_AsScientist_ReturnsOk()
    {
        // Arrange
        var context = await _factory.GetDbContextAsync();
        var seed = await DatabaseSeeder.SeedCompleteDatabaseAsync(context);
        var client = _client.WithScientistUser(seed.ScientistId);

        // Act - Unassign the existing labeler from seed
        var response = await client.DeleteAsync($"/api/subject-video-group-assignments/{seed.Assignment.Id}/unassign-labeler/{seed.LabelerId}");

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.OK);

        // Verify labeler was unassigned
        var updatedContext = await _factory.GetDbContextAsync();
        var updatedAssignment = await updatedContext.SubjectVideoGroupAssignments
            .Include(a => a.Labelers)
            .FirstOrDefaultAsync(a => a.Id == seed.Assignment.Id);
        updatedAssignment.Should().NotBeNull();
        updatedAssignment!.Labelers.Should().NotContain(l => l.Id == seed.LabelerId);
    }
}
