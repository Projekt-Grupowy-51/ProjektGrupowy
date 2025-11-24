using FluentAssertions;
using ProjektGrupowy.API.DTOs.Project;
using ProjektGrupowy.IntegrationTests.Helpers;
using ProjektGrupowy.IntegrationTests.Infrastructure;
using System.Net;
using System.Net.Http.Json;

namespace ProjektGrupowy.IntegrationTests.Tests;

public class ProjectControllerTests : IClassFixture<IntegrationTestWebAppFactory>, IAsyncLifetime
{
    private readonly IntegrationTestWebAppFactory _factory;
    private readonly HttpClient _client;

    public ProjectControllerTests(IntegrationTestWebAppFactory factory)
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
    public async Task GetProjects_AsAuthenticated_ReturnsOk()
    {
        // Arrange
        var context = await _factory.GetDbContextAsync();
        var scientistId = Guid.NewGuid().ToString();
        await DatabaseSeeder.SeedProjectAsync(context, scientistId);
        var client = _client.WithScientistUser(scientistId);

        // Act
        var response = await client.GetAsync("/api/projects");

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.OK);
        var projects = await response.Content.ReadFromJsonAsync<List<ProjectResponse>>();
        projects.Should().NotBeNull().And.HaveCountGreaterOrEqualTo(1);
    }

    [Fact]
    public async Task GetProject_WithValidId_ReturnsProject()
    {
        // Arrange
        var context = await _factory.GetDbContextAsync();
        var scientistId = Guid.NewGuid().ToString();
        var project = await DatabaseSeeder.SeedProjectAsync(context, scientistId);
        var client = _client.WithScientistUser(scientistId);

        // Act
        var response = await client.GetAsync($"/api/projects/{project.Id}");

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.OK);
        var returnedProject = await response.Content.ReadFromJsonAsync<ProjectResponse>();
        returnedProject.Should().NotBeNull();
        returnedProject!.Id.Should().Be(project.Id);
        returnedProject.Name.Should().Be(project.Name);
    }

    [Fact]
    public async Task AddProject_WithValidData_ReturnsCreated()
    {
        // Arrange
        var context = await _factory.GetDbContextAsync();
        var scientistId = Guid.NewGuid().ToString();
        await DatabaseSeeder.EnsureUserExistsAsync(context, scientistId);

        var projectRequest = new ProjectRequest
        {
            Name = "New Integration Test Project",
            Description = "Integration test project description",
            Finished = false
        };
        var client = _client.WithScientistUser(scientistId);

        // Act
        var response = await client.PostAsJsonAsync("/api/projects", projectRequest);

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.Created);
        var createdProject = await response.Content.ReadFromJsonAsync<ProjectResponse>();
        createdProject.Should().NotBeNull();
        createdProject!.Name.Should().Be(projectRequest.Name);
    }

    [Fact]
    public async Task UpdateProject_WithValidData_ReturnsNoContent()
    {
        // Arrange
        var context = await _factory.GetDbContextAsync();
        var scientistId = Guid.NewGuid().ToString();
        var project = await DatabaseSeeder.SeedProjectAsync(context, scientistId);
        var updateRequest = new ProjectRequest
        {
            Name = "Updated Project Name",
            Description = "Updated description",
            Finished = true
        };
        var client = _client.WithScientistUser(scientistId);

        // Act
        var response = await client.PutAsJsonAsync($"/api/projects/{project.Id}", updateRequest);

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.NoContent);
    }

    [Fact]
    public async Task DeleteProject_WithValidId_ReturnsNoContent()
    {
        // Arrange
        var context = await _factory.GetDbContextAsync();
        var scientistId = Guid.NewGuid().ToString();
        var project = await DatabaseSeeder.SeedProjectAsync(context, scientistId);
        var client = _client.WithScientistUser(scientistId);

        // Act
        var response = await client.DeleteAsync($"/api/projects/{project.Id}");

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.NoContent);
    }
}
