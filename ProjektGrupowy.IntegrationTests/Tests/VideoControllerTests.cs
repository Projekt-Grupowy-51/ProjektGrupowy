using FluentAssertions;
using ProjektGrupowy.Application.DTOs.Video;
using ProjektGrupowy.IntegrationTests.Helpers;
using ProjektGrupowy.IntegrationTests.Infrastructure;
using System.Net;
using System.Net.Http.Json;

namespace ProjektGrupowy.IntegrationTests.Tests;

public class VideoControllerTests : IClassFixture<IntegrationTestWebAppFactory>, IAsyncLifetime
{
    private readonly IntegrationTestWebAppFactory _factory;
    private readonly HttpClient _client;

    public VideoControllerTests(IntegrationTestWebAppFactory factory)
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
    public async Task GetVideos_AsScientist_ReturnsOk()
    {
        // Arrange
        var context = await _factory.GetDbContextAsync();
        var scientistId = Guid.NewGuid().ToString();
        await DatabaseSeeder.SeedVideoAsync(context, null, scientistId);
        var client = _client.WithScientistUser(scientistId);

        // Act
        var response = await client.GetAsync("/api/videos");

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.OK);
        var videos = await response.Content.ReadFromJsonAsync<List<VideoResponse>>();
        videos.Should().NotBeNull().And.HaveCountGreaterOrEqualTo(1);
    }

    [Fact]
    public async Task GetVideo_WithValidId_ReturnsVideo()
    {
        // Arrange
        var context = await _factory.GetDbContextAsync();
        var scientistId = Guid.NewGuid().ToString();
        var video = await DatabaseSeeder.SeedVideoAsync(context, null, scientistId);
        var client = _client.WithScientistUser(scientistId);

        // Act
        var response = await client.GetAsync($"/api/videos/{video.Id}");

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.OK);
        var returnedVideo = await response.Content.ReadFromJsonAsync<VideoResponse>();
        returnedVideo.Should().NotBeNull();
        returnedVideo!.Id.Should().Be(video.Id);
        returnedVideo.Title.Should().Be(video.Title);
    }

    [Fact]
    public async Task GetVideo_WithInvalidId_ReturnsNotFound()
    {
        // Arrange
        var client = _client.WithScientistUser();

        // Act
        var response = await client.GetAsync("/api/videos/99999");

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.NotFound);
    }

    [Fact]
    public async Task GetVideos_AsLabeler_ReturnsForbidden()
    {
        // Arrange
        var client = _client.WithLabelerUser();

        // Act
        var response = await client.GetAsync("/api/videos");

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.Forbidden);
    }

    [Fact]
    public async Task GetVideos_AsUnauthenticated_ReturnsUnauthorized()
    {
        // Act
        var response = await _client.GetAsync("/api/videos");

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.Unauthorized);
    }

    [Fact]
    public async Task DeleteVideo_WithValidId_ReturnsNoContent()
    {
        // Arrange
        var context = await _factory.GetDbContextAsync();
        var scientistId = Guid.NewGuid().ToString();
        var video = await DatabaseSeeder.SeedVideoAsync(context, null, scientistId);
        var client = _client.WithScientistUser(scientistId);

        // Act
        var response = await client.DeleteAsync($"/api/videos/{video.Id}");

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.NoContent);
    }

    [Fact]
    public async Task DeleteVideo_WithInvalidId_ReturnsNotFound()
    {
        // Arrange
        var client = _client.WithScientistUser();

        // Act
        var response = await client.DeleteAsync("/api/videos/99999");

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.NotFound);
    }

    [Fact]
    public async Task DeleteVideo_AsLabeler_ReturnsForbidden()
    {
        // Arrange
        var context = await _factory.GetDbContextAsync();
        var video = await DatabaseSeeder.SeedVideoAsync(context);
        var client = _client.WithLabelerUser();

        // Act
        var response = await client.DeleteAsync($"/api/videos/{video.Id}");

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.Forbidden);
    }

    [Fact]
    public async Task GetVideosByBatch_WithValidData_ReturnsVideos()
    {
        // Arrange
        var context = await _factory.GetDbContextAsync();
        var scientistId = Guid.NewGuid().ToString();
        var video = await DatabaseSeeder.SeedVideoAsync(context, null, scientistId);
        var client = _client.WithScientistUser(scientistId);

        // Act
        var response = await client.GetAsync($"/api/videos/batch/{video.VideoGroup.Id}/{video.PositionInQueue}");

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.OK);
        var videos = await response.Content.ReadFromJsonAsync<List<VideoResponse>>();
        videos.Should().NotBeNull().And.HaveCountGreaterOrEqualTo(1);
    }
}
