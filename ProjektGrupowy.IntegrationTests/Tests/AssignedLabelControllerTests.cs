using FluentAssertions;
using ProjektGrupowy.API.DTOs.AssignedLabel;
using ProjektGrupowy.IntegrationTests.Helpers;
using ProjektGrupowy.IntegrationTests.Infrastructure;
using System.Net;
using System.Net.Http.Json;

namespace ProjektGrupowy.IntegrationTests.Tests;

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
    public async Task GetAssignedLabels_AsAuthenticated_ReturnsOk()
    {
        // Arrange
        var context = await _factory.GetDbContextAsync();
        var labelerId = Guid.NewGuid().ToString();
        await DatabaseSeeder.SeedAssignedLabelAsync(context, null, null, labelerId);
        var client = _client.WithLabelerUser(labelerId);

        // Act
        var response = await client.GetAsync("/api/assigned-labels");

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.OK);
        var assignedLabels = await response.Content.ReadFromJsonAsync<List<AssignedLabelResponse>>();
        assignedLabels.Should().NotBeNull();
    }

    [Fact]
    public async Task GetAssignedLabel_WithValidId_ReturnsAssignedLabel()
    {
        // Arrange
        var context = await _factory.GetDbContextAsync();
        var labelerId = Guid.NewGuid().ToString();
        var assignedLabel = await DatabaseSeeder.SeedAssignedLabelAsync(context, null, null, labelerId);
        var client = _client.WithLabelerUser(labelerId);

        // Act
        var response = await client.GetAsync($"/api/assigned-labels/{assignedLabel.Id}");

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.OK);
        var returnedLabel = await response.Content.ReadFromJsonAsync<AssignedLabelResponse>();
        returnedLabel.Should().NotBeNull();
        returnedLabel!.Id.Should().Be(assignedLabel.Id);
    }

    [Fact]
    public async Task AddAssignedLabel_WithValidData_ReturnsCreated()
    {
        // Arrange
        var context = await _factory.GetDbContextAsync();
        var labelerId = Guid.NewGuid().ToString();

        // Ensure labeler user exists in database
        await DatabaseSeeder.EnsureUserExistsAsync(context, labelerId);

        var label = await DatabaseSeeder.SeedLabelAsync(context, null, labelerId);
        var video = await DatabaseSeeder.SeedVideoAsync(context, null, labelerId);

        var assignedLabelRequest = new AssignedLabelRequest
        {
            LabelId = label.Id,
            VideoId = video.Id,
            Start = "5.5",
            End = "15.8"
        };
        var client = _client.WithLabelerUser(labelerId);

        // Act
        var response = await client.PostAsJsonAsync("/api/assigned-labels", assignedLabelRequest);

        // Debug: Print error if not Created
        if (response.StatusCode != HttpStatusCode.Created)
        {
            var errorContent = await response.Content.ReadAsStringAsync();
            Console.WriteLine($"Error: {response.StatusCode} - {errorContent}");
        }

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.Created);
        var createdLabel = await response.Content.ReadFromJsonAsync<AssignedLabelResponse>();
        createdLabel.Should().NotBeNull();
        createdLabel!.LabelId.Should().Be(label.Id);
        createdLabel.VideoId.Should().Be(video.Id);
    }

    [Fact]
    public async Task DeleteAssignedLabel_WithValidId_ReturnsNoContent()
    {
        // Arrange
        var context = await _factory.GetDbContextAsync();
        var labelerId = Guid.NewGuid().ToString();
        var assignedLabel = await DatabaseSeeder.SeedAssignedLabelAsync(context, null, null, labelerId);
        var client = _client.WithLabelerUser(labelerId);

        // Act
        var response = await client.DeleteAsync($"/api/assigned-labels/{assignedLabel.Id}");

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.NoContent);
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
