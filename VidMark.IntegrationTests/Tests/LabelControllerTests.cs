using FluentAssertions;
using VidMark.API.DTOs.Label;
using VidMark.IntegrationTests.Helpers;
using VidMark.IntegrationTests.Infrastructure;
using System.Net;
using System.Net.Http.Json;

namespace VidMark.IntegrationTests.Tests;

public class LabelControllerTests : IClassFixture<IntegrationTestWebAppFactory>, IAsyncLifetime
{
    private readonly IntegrationTestWebAppFactory _factory;
    private readonly HttpClient _client;

    public LabelControllerTests(IntegrationTestWebAppFactory factory)
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
    public async Task GetLabels_AsScientist_ReturnsOwnLabels()
    {
        // Arrange
        var context = await _factory.GetDbContextAsync();
        var seed = await DatabaseSeeder.SeedCompleteDatabaseAsync(context);
        var client = _client.WithScientistUser(seed.ScientistId);

        // Act
        var response = await client.GetAsync("/api/labels");

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.OK);
        var labels = await response.Content.ReadFromJsonAsync<List<LabelResponse>>();
        labels.Should().NotBeNull().And.HaveCount(1);
        labels![0].Id.Should().Be(seed.Label.Id);
        labels[0].Name.Should().Be(seed.Label.Name);
    }

    [Fact]
    public async Task GetLabel_WithValidId_ReturnsLabelWithDetails()
    {
        // Arrange
        var context = await _factory.GetDbContextAsync();
        var seed = await DatabaseSeeder.SeedCompleteDatabaseAsync(context);
        var client = _client.WithScientistUser(seed.ScientistId);

        // Act
        var response = await client.GetAsync($"/api/labels/{seed.Label.Id}");

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.OK);
        var returnedLabel = await response.Content.ReadFromJsonAsync<LabelResponse>();
        returnedLabel.Should().NotBeNull();
        returnedLabel!.Id.Should().Be(seed.Label.Id);
        returnedLabel.Name.Should().Be(seed.Label.Name);
        returnedLabel.ColorHex.Should().Be(seed.Label.ColorHex);
        returnedLabel.SubjectId.Should().Be(seed.Subject.Id);
    }

    [Fact]
    public async Task GetLabel_WithInvalidId_ReturnsNotFound()
    {
        // Arrange
        var context = await _factory.GetDbContextAsync();
        var seed = await DatabaseSeeder.SeedCompleteDatabaseAsync(context);
        var client = _client.WithScientistUser(seed.ScientistId);

        // Act
        var response = await client.GetAsync("/api/labels/99999");

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.NotFound);
    }

    [Fact]
    public async Task AddLabel_WithValidData_ReturnsCreated()
    {
        // Arrange
        var context = await _factory.GetDbContextAsync();
        var seed = await DatabaseSeeder.SeedCompleteDatabaseAsync(context);

        var labelRequest = new LabelRequest
        {
            Name = "New Test Label",
            ColorHex = "#00FF00",
            Type = "point",
            Shortcut = 'N',
            SubjectId = seed.Subject.Id
        };
        var client = _client.WithScientistUser(seed.ScientistId);

        // Act
        var response = await client.PostAsJsonAsync("/api/labels", labelRequest);

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.Created);
        response.Headers.Location.Should().NotBeNull();

        var createdLabel = await response.Content.ReadFromJsonAsync<LabelResponse>();
        createdLabel.Should().NotBeNull();
        createdLabel!.Name.Should().Be(labelRequest.Name);
        createdLabel.ColorHex.Should().Be(labelRequest.ColorHex);
        createdLabel.SubjectId.Should().Be(seed.Subject.Id);
    }

    [Fact]
    public async Task AddLabel_AsLabeler_ReturnsForbidden()
    {
        // Arrange
        var context = await _factory.GetDbContextAsync();
        var seed = await DatabaseSeeder.SeedCompleteDatabaseAsync(context);

        var labelRequest = new LabelRequest
        {
            Name = "Test Label",
            ColorHex = "#00FF00",
            Type = "point",
            Shortcut = 'T',
            SubjectId = seed.Subject.Id
        };
        var client = _client.WithLabelerUser(seed.LabelerId);

        // Act
        var response = await client.PostAsJsonAsync("/api/labels", labelRequest);

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.Forbidden);
    }

    [Fact]
    public async Task UpdateLabel_WithValidData_ReturnsNoContent()
    {
        // Arrange
        var context = await _factory.GetDbContextAsync();
        var seed = await DatabaseSeeder.SeedCompleteDatabaseAsync(context);

        var updateRequest = new LabelRequest
        {
            Name = "Updated Label Name",
            ColorHex = "#0000FF",
            Type = "interval",
            Shortcut = 'U',
            SubjectId = seed.Subject.Id
        };
        var client = _client.WithScientistUser(seed.ScientistId);

        // Act
        var response = await client.PutAsJsonAsync($"/api/labels/{seed.Label.Id}", updateRequest);

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.NoContent);

        // Verify the update
        var getResponse = await client.GetAsync($"/api/labels/{seed.Label.Id}");
        var updatedLabel = await getResponse.Content.ReadFromJsonAsync<LabelResponse>();
        updatedLabel!.Name.Should().Be(updateRequest.Name);
        updatedLabel.ColorHex.Should().Be(updateRequest.ColorHex);
    }

    [Fact]
    public async Task UpdateLabel_AsNonOwner_ReturnsForbidden()
    {
        // Arrange
        var context = await _factory.GetDbContextAsync();
        var seed = await DatabaseSeeder.SeedCompleteDatabaseAsync(context);

        var updateRequest = new LabelRequest
        {
            Name = "Updated Label",
            ColorHex = "#0000FF",
            Type = "point",
            Shortcut = 'U',
            SubjectId = seed.Subject.Id
        };
        var client = _client.WithLabelerUser(seed.LabelerId);

        // Act
        var response = await client.PutAsJsonAsync($"/api/labels/{seed.Label.Id}", updateRequest);

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.Forbidden);
    }

    [Fact]
    public async Task UpdateLabel_WithInvalidId_ReturnsBadRequest()
    {
        // Arrange
        var context = await _factory.GetDbContextAsync();
        var seed = await DatabaseSeeder.SeedCompleteDatabaseAsync(context);

        var updateRequest = new LabelRequest
        {
            Name = "Updated Label",
            ColorHex = "#0000FF",
            Type = "point",
            Shortcut = 'U',
            SubjectId = seed.Subject.Id
        };
        var client = _client.WithScientistUser(seed.ScientistId);

        // Act
        var response = await client.PutAsJsonAsync("/api/labels/99999", updateRequest);

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.BadRequest);
    }

    [Fact]
    public async Task DeleteLabel_WithValidId_ReturnsNoContent()
    {
        // Arrange
        var context = await _factory.GetDbContextAsync();
        var seed = await DatabaseSeeder.SeedCompleteDatabaseAsync(context);
        var client = _client.WithScientistUser(seed.ScientistId);

        // Act
        var response = await client.DeleteAsync($"/api/labels/{seed.Label.Id}");

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.NoContent);

        // Verify deletion (soft delete)
        var getResponse = await client.GetAsync($"/api/labels/{seed.Label.Id}");
        getResponse.StatusCode.Should().Be(HttpStatusCode.NotFound);
    }

    [Fact]
    public async Task DeleteLabel_AsLabeler_ReturnsForbidden()
    {
        // Arrange
        var context = await _factory.GetDbContextAsync();
        var seed = await DatabaseSeeder.SeedCompleteDatabaseAsync(context);
        var client = _client.WithLabelerUser(seed.LabelerId);

        // Act
        var response = await client.DeleteAsync($"/api/labels/{seed.Label.Id}");

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.Forbidden);
    }

    [Fact]
    public async Task DeleteLabel_WithInvalidId_ReturnsNotFound()
    {
        // Arrange
        var context = await _factory.GetDbContextAsync();
        var seed = await DatabaseSeeder.SeedCompleteDatabaseAsync(context);
        var client = _client.WithScientistUser(seed.ScientistId);

        // Act
        var response = await client.DeleteAsync("/api/labels/99999");

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.NotFound);
    }

    [Fact]
    public async Task GetLabels_AsLabeler_ReturnsForbidden()
    {
        // Arrange
        var client = _client.WithLabelerUser();

        // Act
        var response = await client.GetAsync("/api/labels");

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.Forbidden);
    }

    [Fact]
    public async Task GetLabels_AsUnauthenticated_ReturnsUnauthorized()
    {
        // Act
        var response = await _client.GetAsync("/api/labels");

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.Unauthorized);
    }
}
