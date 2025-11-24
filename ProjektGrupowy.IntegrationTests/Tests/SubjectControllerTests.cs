using FluentAssertions;
using ProjektGrupowy.API.DTOs.Subject;
using ProjektGrupowy.IntegrationTests.Helpers;
using ProjektGrupowy.IntegrationTests.Infrastructure;
using System.Net;
using System.Net.Http.Json;

namespace ProjektGrupowy.IntegrationTests.Tests;

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
    public async Task GetSubjects_AsScientist_ReturnsOk()
    {
        // Arrange
        var context = await _factory.GetDbContextAsync();
        var scientistId = Guid.NewGuid().ToString();
        await DatabaseSeeder.SeedSubjectAsync(context, null, scientistId);
        var client = _client.WithScientistUser(scientistId);

        // Act
        var response = await client.GetAsync("/api/subjects");

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.OK);
        var subjects = await response.Content.ReadFromJsonAsync<List<SubjectResponse>>();
        subjects.Should().NotBeNull().And.HaveCountGreaterOrEqualTo(1);
    }

    [Fact]
    public async Task GetSubject_WithValidId_ReturnsSubject()
    {
        // Arrange
        var context = await _factory.GetDbContextAsync();
        var scientistId = Guid.NewGuid().ToString();
        var subject = await DatabaseSeeder.SeedSubjectAsync(context, null, scientistId);
        var client = _client.WithScientistUser(scientistId);

        // Act
        var response = await client.GetAsync($"/api/subjects/{subject.Id}");

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.OK);
        var returnedSubject = await response.Content.ReadFromJsonAsync<SubjectResponse>();
        returnedSubject.Should().NotBeNull();
        returnedSubject!.Id.Should().Be(subject.Id);
    }

    [Fact]
    public async Task AddSubject_WithValidData_ReturnsCreated()
    {
        // Arrange
        var context = await _factory.GetDbContextAsync();
        var scientistId = Guid.NewGuid().ToString();
        var project = await DatabaseSeeder.SeedProjectAsync(context, scientistId);
        var subjectRequest = new SubjectRequest
        {
            Name = "New Test Subject",
            ProjectId = project.Id
        };
        var client = _client.WithScientistUser(scientistId);

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
        var scientistId = Guid.NewGuid().ToString();
        var subject = await DatabaseSeeder.SeedSubjectAsync(context, null, scientistId);
        var updateRequest = new SubjectRequest
        {
            Name = "Updated Subject Name",
            ProjectId = subject.Project.Id
        };
        var client = _client.WithScientistUser(scientistId);

        // Act
        var response = await client.PutAsJsonAsync($"/api/subjects/{subject.Id}", updateRequest);

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.NoContent);
    }

    [Fact]
    public async Task DeleteSubject_WithValidId_ReturnsNoContent()
    {
        // Arrange
        var context = await _factory.GetDbContextAsync();
        var scientistId = Guid.NewGuid().ToString();
        var subject = await DatabaseSeeder.SeedSubjectAsync(context, null, scientistId);
        var client = _client.WithScientistUser(scientistId);

        // Act
        var response = await client.DeleteAsync($"/api/subjects/{subject.Id}");

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
}
