using FluentAssertions;
using VidMark.API.DTOs.VideoGroup;
using VidMark.IntegrationTests.Helpers;
using VidMark.IntegrationTests.Infrastructure;
using System.Net;
using System.Net.Http.Json;

namespace VidMark.IntegrationTests.Tests;

public class VideoGroupControllerTests : IClassFixture<IntegrationTestWebAppFactory>, IAsyncLifetime
{
    private readonly IntegrationTestWebAppFactory _factory;
    private readonly HttpClient _client;

    public VideoGroupControllerTests(IntegrationTestWebAppFactory factory)
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
    public async Task GetVideoGroups_AsScientist_ReturnsOwnVideoGroups()
    {
        // Arrange
        var context = await _factory.GetDbContextAsync();
        var seed = await DatabaseSeeder.SeedCompleteDatabaseAsync(context);
        var client = _client.WithScientistUser(seed.ScientistId);

        // Act
        var response = await client.GetAsync("/api/video-groups");

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.OK);
        var videoGroups = await response.Content.ReadFromJsonAsync<List<VideoGroupResponse>>();
        videoGroups.Should().NotBeNull().And.HaveCount(1);
        videoGroups![0].Id.Should().Be(seed.VideoGroup.Id);
        videoGroups[0].Name.Should().Be(seed.VideoGroup.Name);
    }

    [Fact]
    public async Task GetVideoGroup_WithValidId_ReturnsVideoGroupWithDetails()
    {
        // Arrange
        var context = await _factory.GetDbContextAsync();
        var seed = await DatabaseSeeder.SeedCompleteDatabaseAsync(context);
        var client = _client.WithScientistUser(seed.ScientistId);

        // Act
        var response = await client.GetAsync($"/api/video-groups/{seed.VideoGroup.Id}");

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.OK);
        var returnedVideoGroup = await response.Content.ReadFromJsonAsync<VideoGroupResponse>();
        returnedVideoGroup.Should().NotBeNull();
        returnedVideoGroup!.Id.Should().Be(seed.VideoGroup.Id);
        returnedVideoGroup.Name.Should().Be(seed.VideoGroup.Name);
        returnedVideoGroup.ProjectId.Should().Be(seed.Project.Id);
    }

    [Fact]
    public async Task GetVideoGroup_WithInvalidId_ReturnsNotFound()
    {
        // Arrange
        var context = await _factory.GetDbContextAsync();
        var seed = await DatabaseSeeder.SeedCompleteDatabaseAsync(context);
        var client = _client.WithScientistUser(seed.ScientistId);

        // Act
        var response = await client.GetAsync("/api/video-groups/99999");

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.NotFound);
    }

    [Fact]
    public async Task AddVideoGroup_WithValidData_ReturnsCreated()
    {
        // Arrange
        var context = await _factory.GetDbContextAsync();
        var seed = await DatabaseSeeder.SeedCompleteDatabaseAsync(context);

        var videoGroupRequest = new VideoGroupRequest
        {
            Name = "New Test Video Group",
            ProjectId = seed.Project.Id
        };
        var client = _client.WithScientistUser(seed.ScientistId);

        // Act
        var response = await client.PostAsJsonAsync("/api/video-groups", videoGroupRequest);

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.Created);
        response.Headers.Location.Should().NotBeNull();

        var createdVideoGroup = await response.Content.ReadFromJsonAsync<VideoGroupResponse>();
        createdVideoGroup.Should().NotBeNull();
        createdVideoGroup!.Name.Should().Be(videoGroupRequest.Name);
        createdVideoGroup.ProjectId.Should().Be(seed.Project.Id);
    }

    [Fact]
    public async Task AddVideoGroup_AsLabeler_ReturnsForbidden()
    {
        // Arrange
        var context = await _factory.GetDbContextAsync();
        var seed = await DatabaseSeeder.SeedCompleteDatabaseAsync(context);

        var videoGroupRequest = new VideoGroupRequest
        {
            Name = "Test Video Group",
            ProjectId = seed.Project.Id
        };
        var client = _client.WithLabelerUser(seed.LabelerId);

        // Act
        var response = await client.PostAsJsonAsync("/api/video-groups", videoGroupRequest);

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.Forbidden);
    }

    [Fact]
    public async Task UpdateVideoGroup_WithValidData_ReturnsNoContent()
    {
        // Arrange
        var context = await _factory.GetDbContextAsync();
        var seed = await DatabaseSeeder.SeedCompleteDatabaseAsync(context);

        var updateRequest = new VideoGroupRequest
        {
            Name = "Updated Video Group Name",
            ProjectId = seed.Project.Id
        };
        var client = _client.WithScientistUser(seed.ScientistId);

        // Act
        var response = await client.PutAsJsonAsync($"/api/video-groups/{seed.VideoGroup.Id}", updateRequest);

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.NoContent);

        // Verify update
        var getResponse = await client.GetAsync($"/api/video-groups/{seed.VideoGroup.Id}");
        var updatedVideoGroup = await getResponse.Content.ReadFromJsonAsync<VideoGroupResponse>();
        updatedVideoGroup!.Name.Should().Be(updateRequest.Name);
    }

    [Fact]
    public async Task UpdateVideoGroup_AsNonOwner_ReturnsForbidden()
    {
        // Arrange
        var context = await _factory.GetDbContextAsync();
        var seed = await DatabaseSeeder.SeedCompleteDatabaseAsync(context);

        var updateRequest = new VideoGroupRequest
        {
            Name = "Updated Video Group Name",
            ProjectId = seed.Project.Id
        };
        var client = _client.WithLabelerUser(seed.LabelerId);

        // Act
        var response = await client.PutAsJsonAsync($"/api/video-groups/{seed.VideoGroup.Id}", updateRequest);

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.Forbidden);
    }

    [Fact]
    public async Task UpdateVideoGroup_WithInvalidId_ReturnsBadRequest()
    {
        // Arrange
        var context = await _factory.GetDbContextAsync();
        var seed = await DatabaseSeeder.SeedCompleteDatabaseAsync(context);

        var updateRequest = new VideoGroupRequest
        {
            Name = "Updated Video Group Name",
            ProjectId = seed.Project.Id
        };
        var client = _client.WithScientistUser(seed.ScientistId);

        // Act
        var response = await client.PutAsJsonAsync("/api/video-groups/99999", updateRequest);

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.BadRequest);
    }

    [Fact]
    public async Task DeleteVideoGroup_WithValidId_ReturnsNoContent()
    {
        // Arrange
        var context = await _factory.GetDbContextAsync();
        var seed = await DatabaseSeeder.SeedCompleteDatabaseAsync(context);
        var client = _client.WithScientistUser(seed.ScientistId);

        // Act
        var response = await client.DeleteAsync($"/api/video-groups/{seed.VideoGroup.Id}");

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.NoContent);

        // Verify deletion (soft delete)
        var getResponse = await client.GetAsync($"/api/video-groups/{seed.VideoGroup.Id}");
        getResponse.StatusCode.Should().Be(HttpStatusCode.NotFound);
    }

    [Fact]
    public async Task DeleteVideoGroup_AsLabeler_ReturnsForbidden()
    {
        // Arrange
        var context = await _factory.GetDbContextAsync();
        var seed = await DatabaseSeeder.SeedCompleteDatabaseAsync(context);
        var client = _client.WithLabelerUser(seed.LabelerId);

        // Act
        var response = await client.DeleteAsync($"/api/video-groups/{seed.VideoGroup.Id}");

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.Forbidden);
    }

    [Fact]
    public async Task DeleteVideoGroup_WithInvalidId_ReturnsNotFound()
    {
        // Arrange
        var context = await _factory.GetDbContextAsync();
        var seed = await DatabaseSeeder.SeedCompleteDatabaseAsync(context);
        var client = _client.WithScientistUser(seed.ScientistId);

        // Act
        var response = await client.DeleteAsync("/api/video-groups/99999");

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.NotFound);
    }

    [Fact]
    public async Task GetVideoGroups_AsLabeler_ReturnsForbidden()
    {
        // Arrange
        var client = _client.WithLabelerUser();

        // Act
        var response = await client.GetAsync("/api/video-groups");

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.Forbidden);
    }

    [Fact]
    public async Task GetVideoGroups_AsUnauthenticated_ReturnsUnauthorized()
    {
        // Act
        var response = await _client.GetAsync("/api/video-groups");

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.Unauthorized);
    }

    [Fact]
    public async Task GetVideoGroup_AsLabelerWithAssignment_ReturnsVideoGroup()
    {
        // Arrange
        var context = await _factory.GetDbContextAsync();
        var seed = await DatabaseSeeder.SeedCompleteDatabaseAsync(context);
        var client = _client.WithLabelerUser(seed.LabelerId);

        // Act
        var response = await client.GetAsync($"/api/video-groups/{seed.VideoGroup.Id}");

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.OK);
        var returnedVideoGroup = await response.Content.ReadFromJsonAsync<VideoGroupResponse>();
        returnedVideoGroup.Should().NotBeNull();
        returnedVideoGroup!.Id.Should().Be(seed.VideoGroup.Id);
    }
}
