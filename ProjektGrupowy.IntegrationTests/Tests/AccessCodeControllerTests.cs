using FluentAssertions;
using ProjektGrupowy.Application.DTOs.AccessCode;
using ProjektGrupowy.Domain.Enums;
using ProjektGrupowy.IntegrationTests.Helpers;
using ProjektGrupowy.IntegrationTests.Infrastructure;
using System.Net;
using System.Net.Http.Json;

namespace ProjektGrupowy.IntegrationTests.Tests;

public class AccessCodeControllerTests : IClassFixture<IntegrationTestWebAppFactory>, IAsyncLifetime
{
    private readonly IntegrationTestWebAppFactory _factory;
    private readonly HttpClient _client;

    public AccessCodeControllerTests(IntegrationTestWebAppFactory factory)
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
    public async Task GetAccessCodesByProject_AsScientist_ReturnsOk()
    {
        // Arrange
        var context = await _factory.GetDbContextAsync();
        var scientistId = Guid.NewGuid().ToString();
        var project = await DatabaseSeeder.SeedProjectAsync(context, scientistId);
        var client = _client.WithScientistUser(scientistId);

        // Act
        var response = await client.GetAsync($"/api/access-codes/projects/{project.Id}");

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.OK);
        var accessCodes = await response.Content.ReadFromJsonAsync<List<AccessCodeResponse>>();
        accessCodes.Should().NotBeNull();
    }

    [Fact]
    public async Task GetAccessCodesByProject_WithInvalidProjectId_ReturnsNotFound()
    {
        // Arrange
        var client = _client.WithScientistUser();

        // Act
        var response = await client.GetAsync("/api/access-codes/projects/99999");

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.NotFound);
    }

    [Fact]
    public async Task GetAccessCodesByProject_AsLabeler_ReturnsForbidden()
    {
        // Arrange
        var context = await _factory.GetDbContextAsync();
        var project = await DatabaseSeeder.SeedProjectAsync(context);
        var client = _client.WithLabelerUser();

        // Act
        var response = await client.GetAsync($"/api/access-codes/projects/{project.Id}");

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.Forbidden);
    }

    [Fact]
    public async Task GetAccessCodesByProject_AsUnauthenticated_ReturnsUnauthorized()
    {
        // Act
        var response = await _client.GetAsync("/api/access-codes/projects/1");

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.Unauthorized);
    }

    [Fact]
    public async Task AddValidCodeToProject_WithValidData_ReturnsCreated()
    {
        // Arrange
        var context = await _factory.GetDbContextAsync();
        var scientistId = Guid.NewGuid().ToString();

        var project = await DatabaseSeeder.SeedProjectAsync(context, scientistId);
        var createRequest = new CreateAccessCodeRequest
        {
            ProjectId = project.Id,
            Expiration = AccessCodeExpiration.Custom,
            CustomExpiration = 10
        };
        var client = _client.WithScientistUser(scientistId);

        // Act
        var response = await client.PostAsJsonAsync("/api/access-codes/project", createRequest);

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.Created);
        var createdCode = await response.Content.ReadFromJsonAsync<AccessCodeResponse>();
        createdCode.Should().NotBeNull();
        createdCode!.ProjectId.Should().Be(project.Id);
    }

    [Fact]
    public async Task AddValidCodeToProject_AsLabeler_ReturnsForbidden()
    {
        // Arrange
        var createRequest = new CreateAccessCodeRequest
        {
            ProjectId = 1,
            Expiration = AccessCodeExpiration.In30Days
        };
        var client = _client.WithLabelerUser();

        // Act
        var response = await client.PostAsJsonAsync("/api/access-codes/project", createRequest);

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.Forbidden);
    }

    [Fact]
    public async Task ValidateAccessCode_WithValidCode_ReturnsTrue()
    {
        // Arrange
        var context = await _factory.GetDbContextAsync();
        var scientistId = Guid.NewGuid().ToString();
        var project = await DatabaseSeeder.SeedProjectAsync(context, scientistId);
        var accessCode = await DatabaseSeeder.SeedAccessCodeAsync(context, projectId: project.Id, userId: scientistId);
        var validateRequest = new AccessCodeRequest { Code = accessCode.Code };
        var client = _client.WithScientistUser(scientistId);

        // Act
        var response = await client.PostAsJsonAsync("/api/access-codes/validate", validateRequest);

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.OK);
        var isValid = await response.Content.ReadFromJsonAsync<bool>();
        isValid.Should().BeTrue();
    }

    [Fact]
    public async Task ValidateAccessCode_WithInvalidCode_ReturnsFalse()
    {
        // Arrange
        var validateRequest = new AccessCodeRequest { Code = "INVALID_CODE1234" }; // 16 characters
        var client = _client.WithScientistUser();

        // Act
        var response = await client.PostAsJsonAsync("/api/access-codes/validate", validateRequest);

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.OK);
        var isValid = await response.Content.ReadFromJsonAsync<bool>();
        isValid.Should().BeFalse();
    }
}
