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
    public async Task GetProjects_AsScientist_ReturnsOwnProjects()
    {
        // Arrange
        var context = await _factory.GetDbContextAsync();
        var seed = await DatabaseSeeder.SeedCompleteDatabaseAsync(context);
        var client = _client.WithScientistUser(seed.ScientistId);

        // Act
        var response = await client.GetAsync("/api/projects");

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.OK);
        var projects = await response.Content.ReadFromJsonAsync<List<ProjectResponse>>();
        projects.Should().NotBeNull().And.HaveCount(1);
        projects![0].Id.Should().Be(seed.Project.Id);
        projects[0].Name.Should().Be(seed.Project.Name);
    }

    [Fact]
    public async Task GetProject_WithValidId_ReturnsProjectWithRelations()
    {
        // Arrange
        var context = await _factory.GetDbContextAsync();
        var seed = await DatabaseSeeder.SeedCompleteDatabaseAsync(context);
        var client = _client.WithScientistUser(seed.ScientistId);

        // Act
        var response = await client.GetAsync($"/api/projects/{seed.Project.Id}");

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.OK);
        var returnedProject = await response.Content.ReadFromJsonAsync<ProjectResponse>();
        returnedProject.Should().NotBeNull();
        returnedProject!.Id.Should().Be(seed.Project.Id);
        returnedProject.Name.Should().Be(seed.Project.Name);
    }

    [Fact]
    public async Task AddProject_WithValidData_ReturnsCreated()
    {
        // Arrange
        var context = await _factory.GetDbContextAsync();
        var seed = await DatabaseSeeder.SeedCompleteDatabaseAsync(context);

        var projectRequest = new ProjectRequest
        {
            Name = "New Integration Test Project",
            Description = "Integration test project description",
            Finished = false
        };
        var client = _client.WithScientistUser(seed.ScientistId);

        // Act
        var response = await client.PostAsJsonAsync("/api/projects", projectRequest);

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.Created);
        var createdProject = await response.Content.ReadFromJsonAsync<ProjectResponse>();
        createdProject.Should().NotBeNull();
        createdProject!.Name.Should().Be(projectRequest.Name);
        createdProject.Description.Should().Be(projectRequest.Description);
    }

    [Fact]
    public async Task UpdateProject_WithValidData_ReturnsNoContent()
    {
        // Arrange
        var context = await _factory.GetDbContextAsync();
        var seed = await DatabaseSeeder.SeedCompleteDatabaseAsync(context);

        var updateRequest = new ProjectRequest
        {
            Name = "Updated Project Name",
            Description = "Updated description",
            Finished = true
        };
        var client = _client.WithScientistUser(seed.ScientistId);

        // Act
        var response = await client.PutAsJsonAsync($"/api/projects/{seed.Project.Id}", updateRequest);

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.NoContent);

        // Verify update
        var getResponse = await client.GetAsync($"/api/projects/{seed.Project.Id}");
        var updatedProject = await getResponse.Content.ReadFromJsonAsync<ProjectResponse>();
        updatedProject!.Name.Should().Be(updateRequest.Name);
        updatedProject.Description.Should().Be(updateRequest.Description);
    }

    [Fact]
    public async Task UpdateProject_AsNonOwner_ReturnsForbidden()
    {
        // Arrange
        var context = await _factory.GetDbContextAsync();
        var seed = await DatabaseSeeder.SeedCompleteDatabaseAsync(context);

        var updateRequest = new ProjectRequest
        {
            Name = "Updated Project Name",
            Description = "Updated description",
            Finished = true
        };
        var client = _client.WithLabelerUser(seed.LabelerId);

        // Act
        var response = await client.PutAsJsonAsync($"/api/projects/{seed.Project.Id}", updateRequest);

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.Forbidden);
    }

    [Fact]
    public async Task DeleteProject_WithValidId_ReturnsNoContent()
    {
        // Arrange
        var context = await _factory.GetDbContextAsync();
        var seed = await DatabaseSeeder.SeedCompleteDatabaseAsync(context);
        var client = _client.WithScientistUser(seed.ScientistId);

        // Act
        var response = await client.DeleteAsync($"/api/projects/{seed.Project.Id}");

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.NoContent);

        // Verify deletion (soft delete)
        var getResponse = await client.GetAsync($"/api/projects/{seed.Project.Id}");
        getResponse.StatusCode.Should().Be(HttpStatusCode.NotFound);
    }

    [Fact]
    public async Task DeleteProject_AsNonOwner_ReturnsForbidden()
    {
        // Arrange
        var context = await _factory.GetDbContextAsync();
        var seed = await DatabaseSeeder.SeedCompleteDatabaseAsync(context);
        var client = _client.WithLabelerUser(seed.LabelerId);

        // Act
        var response = await client.DeleteAsync($"/api/projects/{seed.Project.Id}");

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.Forbidden);
    }

    [Fact]
    public async Task GetProject_WithInvalidId_ReturnsNotFound()
    {
        // Arrange
        var context = await _factory.GetDbContextAsync();
        var seed = await DatabaseSeeder.SeedCompleteDatabaseAsync(context);
        var client = _client.WithScientistUser(seed.ScientistId);

        // Act
        var response = await client.GetAsync("/api/projects/99999");

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.NotFound);
    }

}
