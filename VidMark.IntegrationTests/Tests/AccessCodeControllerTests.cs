using FluentAssertions;
using VidMark.API.DTOs.AccessCode;
using VidMark.Domain.Enums;
using VidMark.IntegrationTests.Helpers;
using VidMark.IntegrationTests.Infrastructure;
using System.Net;
using System.Net.Http.Json;

namespace VidMark.IntegrationTests.Tests;

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
    public async Task GetAccessCodesByProject_AsScientist_ReturnsOwnAccessCodes()
    {
        // Arrange
        var context = await _factory.GetDbContextAsync();
        var seed = await DatabaseSeeder.SeedCompleteDatabaseAsync(context);
        var client = _client.WithScientistUser(seed.ScientistId);

        // Act
        var response = await client.GetAsync($"/api/access-codes/projects/{seed.Project.Id}");

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.OK);
        var accessCodes = await response.Content.ReadFromJsonAsync<List<AccessCodeResponse>>();
        accessCodes.Should().NotBeNull().And.HaveCount(1);
        accessCodes![0].Id.Should().Be(seed.AccessCode.Id);
        accessCodes[0].ProjectId.Should().Be(seed.Project.Id);
    }

    [Fact]
    public async Task GetAccessCodesByProject_WithInvalidProjectId_ReturnsNotFound()
    {
        // Arrange
        var context = await _factory.GetDbContextAsync();
        var seed = await DatabaseSeeder.SeedCompleteDatabaseAsync(context);
        var client = _client.WithScientistUser(seed.ScientistId);

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
        var seed = await DatabaseSeeder.SeedCompleteDatabaseAsync(context);
        var client = _client.WithLabelerUser(seed.LabelerId);

        // Act
        var response = await client.GetAsync($"/api/access-codes/projects/{seed.Project.Id}");

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
        var seed = await DatabaseSeeder.SeedCompleteDatabaseAsync(context);

        var createRequest = new CreateAccessCodeRequest
        {
            ProjectId = seed.Project.Id,
            Expiration = AccessCodeExpiration.Custom,
            CustomExpiration = 10
        };
        var client = _client.WithScientistUser(seed.ScientistId);

        // Act
        var response = await client.PostAsJsonAsync("/api/access-codes/project", createRequest);

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.Created);
        response.Headers.Location.Should().NotBeNull();

        var createdCode = await response.Content.ReadFromJsonAsync<AccessCodeResponse>();
        createdCode.Should().NotBeNull();
        createdCode!.ProjectId.Should().Be(seed.Project.Id);
        createdCode.Code.Should().NotBeNullOrEmpty();
    }

    [Fact]
    public async Task AddValidCodeToProject_AsLabeler_ReturnsForbidden()
    {
        // Arrange
        var context = await _factory.GetDbContextAsync();
        var seed = await DatabaseSeeder.SeedCompleteDatabaseAsync(context);

        var createRequest = new CreateAccessCodeRequest
        {
            ProjectId = seed.Project.Id,
            Expiration = AccessCodeExpiration.In30Days
        };
        var client = _client.WithLabelerUser(seed.LabelerId);

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
        var seed = await DatabaseSeeder.SeedCompleteDatabaseAsync(context);
        var validateRequest = new AccessCodeRequest { Code = seed.AccessCode.Code };
        var client = _client.WithScientistUser(seed.ScientistId);

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
        var context = await _factory.GetDbContextAsync();
        var seed = await DatabaseSeeder.SeedCompleteDatabaseAsync(context);
        var validateRequest = new AccessCodeRequest { Code = "INVALID_CODE1234" }; // 16 characters
        var client = _client.WithScientistUser(seed.ScientistId);

        // Act
        var response = await client.PostAsJsonAsync("/api/access-codes/validate", validateRequest);

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.OK);
        var isValid = await response.Content.ReadFromJsonAsync<bool>();
        isValid.Should().BeFalse();
    }
}
