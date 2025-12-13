using FluentAssertions;
using VidMark.API.DTOs.AssignedLabel;
using VidMark.IntegrationTests.Helpers;
using VidMark.IntegrationTests.Infrastructure;
using System.Net;
using System.Net.Http.Json;

namespace VidMark.IntegrationTests.Tests;

public class AssignedLabelControllerTests : IClassFixture<IntegrationTestWebAppFactory>, IAsyncLifetime
{
    private readonly IntegrationTestWebAppFactory _factory;
    private readonly HttpClient _client;

    public AssignedLabelControllerTests(IntegrationTestWebAppFactory factory)
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
    public async Task GetAssignedLabels_AsLabeler_ReturnsOk()
    {
        // Arrange
        var context = await _factory.GetDbContextAsync();
        var seed = await DatabaseSeeder.SeedCompleteDatabaseAsync(context);
        var client = _client.WithLabelerUser(seed.LabelerId);

        // Act
        var response = await client.GetAsync("/api/assigned-labels");

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.OK);
        var assignedLabels = await response.Content.ReadFromJsonAsync<List<AssignedLabelResponse>>();
        assignedLabels.Should().NotBeNull();
    }

    [Fact]
    public async Task GetAssignedLabel_WithValidId_ReturnsAssignedLabelWithDetails()
    {
        // Arrange
        var context = await _factory.GetDbContextAsync();
        var seed = await DatabaseSeeder.SeedCompleteDatabaseAsync(context);
        var client = _client.WithLabelerUser(seed.LabelerId);

        // Act
        var response = await client.GetAsync($"/api/assigned-labels/{seed.AssignedLabel.Id}");

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.OK);
        var returnedLabel = await response.Content.ReadFromJsonAsync<AssignedLabelResponse>();
        returnedLabel.Should().NotBeNull();
        returnedLabel!.Id.Should().Be(seed.AssignedLabel.Id);
        returnedLabel.LabelId.Should().Be(seed.Label.Id);
        returnedLabel.VideoId.Should().Be(seed.Video.Id);
    }

    [Fact]
    public async Task GetAssignedLabel_WithInvalidId_ReturnsNotFound()
    {
        // Arrange
        var context = await _factory.GetDbContextAsync();
        var seed = await DatabaseSeeder.SeedCompleteDatabaseAsync(context);
        var client = _client.WithLabelerUser(seed.LabelerId);

        // Act
        var response = await client.GetAsync("/api/assigned-labels/99999");

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.NotFound);
    }

    [Fact]
    public async Task AddAssignedLabel_WithValidData_ReturnsCreated()
    {
        // Arrange
        var context = await _factory.GetDbContextAsync();
        var seed = await DatabaseSeeder.SeedCompleteDatabaseAsync(context);

        var assignedLabelRequest = new AssignedLabelRequest
        {
            LabelId = seed.Label.Id,
            VideoId = seed.Video.Id,
            Start = "20.5",
            End = "25.8"
        };
        var client = _client.WithLabelerUser(seed.LabelerId);

        // Act
        var response = await client.PostAsJsonAsync("/api/assigned-labels", assignedLabelRequest);

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.Created);
        response.Headers.Location.Should().NotBeNull();

        var createdLabel = await response.Content.ReadFromJsonAsync<AssignedLabelResponse>();
        createdLabel.Should().NotBeNull();
        createdLabel!.LabelId.Should().Be(seed.Label.Id);
        createdLabel.VideoId.Should().Be(seed.Video.Id);
        createdLabel.Start.Should().Be("20.5");
        createdLabel.End.Should().Be("25.8");
    }

    [Fact]
    public async Task DeleteAssignedLabel_WithValidId_ReturnsNoContent()
    {
        // Arrange
        var context = await _factory.GetDbContextAsync();
        var seed = await DatabaseSeeder.SeedCompleteDatabaseAsync(context);
        var client = _client.WithLabelerUser(seed.LabelerId);

        // Act
        var response = await client.DeleteAsync($"/api/assigned-labels/{seed.AssignedLabel.Id}");

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.NoContent);

        // Verify deletion (soft delete)
        var getResponse = await client.GetAsync($"/api/assigned-labels/{seed.AssignedLabel.Id}");
        getResponse.StatusCode.Should().Be(HttpStatusCode.NotFound);
    }

    [Fact]
    public async Task DeleteAssignedLabel_WithInvalidId_ReturnsNotFound()
    {
        // Arrange
        var context = await _factory.GetDbContextAsync();
        var seed = await DatabaseSeeder.SeedCompleteDatabaseAsync(context);
        var client = _client.WithLabelerUser(seed.LabelerId);

        // Act
        var response = await client.DeleteAsync("/api/assigned-labels/99999");

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.NotFound);
    }

    [Fact]
    public async Task GetAssignedLabels_AsUnauthenticated_ReturnsUnauthorized()
    {
        // Act
        var response = await _client.GetAsync("/api/assigned-labels");

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.Unauthorized);
    }
}
