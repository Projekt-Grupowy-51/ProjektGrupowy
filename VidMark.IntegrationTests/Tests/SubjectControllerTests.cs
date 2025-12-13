using FluentAssertions;
using VidMark.API.DTOs.Subject;
using VidMark.IntegrationTests.Helpers;
using VidMark.IntegrationTests.Infrastructure;
using System.Net;
using System.Net.Http.Json;

namespace VidMark.IntegrationTests.Tests;

public class SubjectControllerTests : IClassFixture<IntegrationTestWebAppFactory>, IAsyncLifetime
{
    private readonly IntegrationTestWebAppFactory _factory;
    private readonly HttpClient _client;

    public SubjectControllerTests(IntegrationTestWebAppFactory factory)
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
    public async Task GetSubjects_AsScientist_ReturnsOwnSubjects()
    {
        // Arrange
        var context = await _factory.GetDbContextAsync();
        var seed = await DatabaseSeeder.SeedCompleteDatabaseAsync(context);
        var client = _client.WithScientistUser(seed.ScientistId);

        // Act
        var response = await client.GetAsync("/api/subjects");

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.OK);
        var subjects = await response.Content.ReadFromJsonAsync<List<SubjectResponse>>();
        subjects.Should().NotBeNull().And.HaveCount(1);
        subjects![0].Id.Should().Be(seed.Subject.Id);
    }

    [Fact]
    public async Task GetSubject_WithValidId_ReturnsSubjectWithLabels()
    {
        // Arrange
        var context = await _factory.GetDbContextAsync();
        var seed = await DatabaseSeeder.SeedCompleteDatabaseAsync(context);
        var client = _client.WithScientistUser(seed.ScientistId);

        // Act
        var response = await client.GetAsync($"/api/subjects/{seed.Subject.Id}");

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.OK);
        var returnedSubject = await response.Content.ReadFromJsonAsync<SubjectResponse>();
        returnedSubject.Should().NotBeNull();
        returnedSubject!.Id.Should().Be(seed.Subject.Id);
        returnedSubject.Name.Should().Be(seed.Subject.Name);
    }

    [Fact]
    public async Task AddSubject_WithValidData_ReturnsCreated()
    {
        // Arrange
        var context = await _factory.GetDbContextAsync();
        var seed = await DatabaseSeeder.SeedCompleteDatabaseAsync(context);

        var subjectRequest = new SubjectRequest
        {
            Name = "New Test Subject",
            Description = "Test description",
            ProjectId = seed.Project.Id
        };
        var client = _client.WithScientistUser(seed.ScientistId);

        // Act
        var response = await client.PostAsJsonAsync("/api/subjects", subjectRequest);

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.Created);
        var createdSubject = await response.Content.ReadFromJsonAsync<SubjectResponse>();
        createdSubject.Should().NotBeNull();
        createdSubject!.Name.Should().Be(subjectRequest.Name);
    }

    [Fact]
    public async Task UpdateSubject_WithValidData_ReturnsNoContent()
    {
        // Arrange
        var context = await _factory.GetDbContextAsync();
        var seed = await DatabaseSeeder.SeedCompleteDatabaseAsync(context);

        var updateRequest = new SubjectRequest
        {
            Name = "Updated Subject Name",
            Description = "Updated description",
            ProjectId = seed.Project.Id
        };
        var client = _client.WithScientistUser(seed.ScientistId);

        // Act
        var response = await client.PutAsJsonAsync($"/api/subjects/{seed.Subject.Id}", updateRequest);

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.NoContent);
    }

    [Fact]
    public async Task DeleteSubject_WithValidId_ReturnsNoContent()
    {
        // Arrange
        var context = await _factory.GetDbContextAsync();
        var seed = await DatabaseSeeder.SeedCompleteDatabaseAsync(context);
        var client = _client.WithScientistUser(seed.ScientistId);

        // Act
        var response = await client.DeleteAsync($"/api/subjects/{seed.Subject.Id}");

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.NoContent);
    }

    [Fact]
    public async Task GetSubjects_AsLabeler_ReturnsForbidden()
    {
        // Arrange
        var client = _client.WithLabelerUser();

        // Act
        var response = await client.GetAsync("/api/subjects");

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.Forbidden);
    }

    [Fact]
    public async Task GetSubject_WithInvalidId_ReturnsNotFound()
    {
        // Arrange
        var context = await _factory.GetDbContextAsync();
        var seed = await DatabaseSeeder.SeedCompleteDatabaseAsync(context);
        var client = _client.WithScientistUser(seed.ScientistId);

        // Act
        var response = await client.GetAsync("/api/subjects/99999");

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.NotFound);
    }
}
