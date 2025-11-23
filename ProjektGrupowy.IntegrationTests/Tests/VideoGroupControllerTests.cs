using FluentAssertions;
using ProjektGrupowy.Application.DTOs.VideoGroup;
using ProjektGrupowy.IntegrationTests.Helpers;
using ProjektGrupowy.IntegrationTests.Infrastructure;
using System.Net;
using System.Net.Http.Json;

namespace ProjektGrupowy.IntegrationTests.Tests;

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
    public async Task GetVideoGroups_AsScientist_ReturnsOk()
    {
        // Arrange
        var context = await _factory.GetDbContextAsync();
        var scientistId = Guid.NewGuid().ToString();
        await DatabaseSeeder.SeedVideoGroupAsync(context, null, scientistId);
        var client = _client.WithScientistUser(scientistId);

        // Act
        var response = await client.GetAsync("/api/video-groups");

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.OK);
        var videoGroups = await response.Content.ReadFromJsonAsync<List<VideoGroupResponse>>();
        videoGroups.Should().NotBeNull().And.HaveCountGreaterOrEqualTo(1);
    }

    [Fact]
    public async Task GetVideoGroup_WithValidId_ReturnsVideoGroup()
    {
        // Arrange
        var context = await _factory.GetDbContextAsync();
        var scientistId = Guid.NewGuid().ToString();
        var videoGroup = await DatabaseSeeder.SeedVideoGroupAsync(context, null, scientistId);
        var client = _client.WithScientistUser(scientistId);

        // Act
        var response = await client.GetAsync($"/api/video-groups/{videoGroup.Id}");

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.OK);
        var returnedVideoGroup = await response.Content.ReadFromJsonAsync<VideoGroupResponse>();
        returnedVideoGroup.Should().NotBeNull();
        returnedVideoGroup!.Id.Should().Be(videoGroup.Id);
    }

    [Fact]
    public async Task AddVideoGroup_WithValidData_ReturnsCreated()
    {
        // Arrange
        var context = await _factory.GetDbContextAsync();
        var scientistId = Guid.NewGuid().ToString();
        var project = await DatabaseSeeder.SeedProjectAsync(context, scientistId);
        var videoGroupRequest = new VideoGroupRequest
        {
            Name = "New Test Video Group",
            ProjectId = project.Id
        };
        var client = _client.WithScientistUser(scientistId);

        // Act
        var response = await client.PostAsJsonAsync("/api/video-groups", videoGroupRequest);

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.Created);
        var createdVideoGroup = await response.Content.ReadFromJsonAsync<VideoGroupResponse>();
        createdVideoGroup.Should().NotBeNull();
        createdVideoGroup!.Name.Should().Be(videoGroupRequest.Name);
    }

    [Fact]
    public async Task UpdateVideoGroup_WithValidData_ReturnsNoContent()
    {
        // Arrange
        var context = await _factory.GetDbContextAsync();
        var scientistId = Guid.NewGuid().ToString();
        var videoGroup = await DatabaseSeeder.SeedVideoGroupAsync(context, null, scientistId);
        var updateRequest = new VideoGroupRequest
        {
            Name = "Updated Video Group Name",
            ProjectId = videoGroup.Project.Id
        };
        var client = _client.WithScientistUser(scientistId);

        // Act
        var response = await client.PutAsJsonAsync($"/api/video-groups/{videoGroup.Id}", updateRequest);

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.NoContent);
    }

    [Fact]
    public async Task DeleteVideoGroup_WithValidId_ReturnsNoContent()
    {
        // Arrange
        var context = await _factory.GetDbContextAsync();
        var scientistId = Guid.NewGuid().ToString();
        var videoGroup = await DatabaseSeeder.SeedVideoGroupAsync(context, null, scientistId);
        var client = _client.WithScientistUser(scientistId);

        // Act
        var response = await client.DeleteAsync($"/api/video-groups/{videoGroup.Id}");

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.NoContent);
    }
}
