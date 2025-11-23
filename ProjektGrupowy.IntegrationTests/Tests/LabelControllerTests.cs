using FluentAssertions;
using ProjektGrupowy.Application.DTOs.Label;
using ProjektGrupowy.IntegrationTests.Helpers;
using ProjektGrupowy.IntegrationTests.Infrastructure;
using System.Net;
using System.Net.Http.Json;

namespace ProjektGrupowy.IntegrationTests.Tests;

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
        // Clean database before each test
        var context = await _factory.GetDbContextAsync();
        await DatabaseSeeder.ClearDatabaseAsync(context);
    }

    public Task DisposeAsync() => Task.CompletedTask;

    [Fact]
    public async Task GetLabels_AsScientist_ReturnsOkWithLabels()
    {
        // Arrange
        var context = await _factory.GetDbContextAsync();
        var scientistId = Guid.NewGuid().ToString();
        var subject = await DatabaseSeeder.SeedSubjectAsync(context, null, scientistId);
        var label1 = await DatabaseSeeder.SeedLabelAsync(context, subject.Id, scientistId);
        var label2 = await DatabaseSeeder.SeedLabelAsync(context, subject.Id, scientistId);

        var client = _client.WithScientistUser(scientistId);

        // Act
        var response = await client.GetAsync("/api/labels");

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.OK);
        var labels = await response.Content.ReadFromJsonAsync<List<LabelResponse>>();
        labels.Should().NotBeNull();
        labels.Should().HaveCountGreaterOrEqualTo(2);
        labels.Should().Contain(l => l.Id == label1.Id);
        labels.Should().Contain(l => l.Id == label2.Id);
    }

    [Fact]
    public async Task GetLabels_AsAdmin_ReturnsOkWithAllLabels()
    {
        // Arrange
        var context = await _factory.GetDbContextAsync();
        var adminId = Guid.NewGuid().ToString();
        var subject = await DatabaseSeeder.SeedSubjectAsync(context, null, adminId);
        await DatabaseSeeder.SeedLabelAsync(context, subject.Id, adminId);

        var client = _client.WithAdminUser(adminId);

        // Act
        var response = await client.GetAsync("/api/labels");

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.OK);
        var labels = await response.Content.ReadFromJsonAsync<List<LabelResponse>>();
        labels.Should().NotBeNull();
        labels.Should().HaveCountGreaterOrEqualTo(1);
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

    [Fact]
    public async Task GetLabel_WithValidId_ReturnsLabel()
    {
        // Arrange
        var context = await _factory.GetDbContextAsync();
        var scientistId = Guid.NewGuid().ToString();
        var label = await DatabaseSeeder.SeedLabelAsync(context, null, scientistId);

        var client = _client.WithScientistUser(scientistId);

        // Act
        var response = await client.GetAsync($"/api/labels/{label.Id}");

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.OK);
        var returnedLabel = await response.Content.ReadFromJsonAsync<LabelResponse>();
        returnedLabel.Should().NotBeNull();
        returnedLabel!.Id.Should().Be(label.Id);
        returnedLabel.Name.Should().Be(label.Name);
        returnedLabel.ColorHex.Should().Be(label.ColorHex);
    }

    [Fact]
    public async Task GetLabel_WithInvalidId_ReturnsNotFound()
    {
        // Arrange
        var client = _client.WithScientistUser();

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
        var scientistId = Guid.NewGuid().ToString();
        var subject = await DatabaseSeeder.SeedSubjectAsync(context, null, scientistId);

        var labelRequest = new LabelRequest
        {
            Name = "New Test Label",
            ColorHex = "#00FF00",
            Type = "point",
            Shortcut = 'N',
            SubjectId = subject.Id
        };

        var client = _client.WithScientistUser(scientistId);

        // Act
        var response = await client.PostAsJsonAsync("/api/labels", labelRequest);

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.Created);
        response.Headers.Location.Should().NotBeNull();

        var createdLabel = await response.Content.ReadFromJsonAsync<LabelResponse>();
        createdLabel.Should().NotBeNull();
        createdLabel!.Name.Should().Be(labelRequest.Name);
        createdLabel.ColorHex.Should().Be(labelRequest.ColorHex);
        createdLabel.SubjectId.Should().Be(subject.Id);
    }

    [Fact]
    public async Task AddLabel_AsLabeler_ReturnsForbidden()
    {
        // Arrange
        var labelRequest = new LabelRequest
        {
            Name = "Test Label",
            ColorHex = "#00FF00",
            Type = "point",
            Shortcut = 'T',
            SubjectId = 1
        };

        var client = _client.WithLabelerUser();

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
        var scientistId = Guid.NewGuid().ToString();
        var label = await DatabaseSeeder.SeedLabelAsync(context, null, scientistId);

        var updateRequest = new LabelRequest
        {
            Name = "Updated Label Name",
            ColorHex = "#0000FF",
            Type = "interval",
            Shortcut = 'U',
            SubjectId = label.Subject.Id
        };

        var client = _client.WithScientistUser(scientistId);

        // Act
        var response = await client.PutAsJsonAsync($"/api/labels/{label.Id}", updateRequest);

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.NoContent);

        // Verify the update
        var getResponse = await client.GetAsync($"/api/labels/{label.Id}");
        var updatedLabel = await getResponse.Content.ReadFromJsonAsync<LabelResponse>();
        updatedLabel.Should().NotBeNull();
        updatedLabel!.Name.Should().Be(updateRequest.Name);
        updatedLabel.ColorHex.Should().Be(updateRequest.ColorHex);
    }

    [Fact]
    public async Task UpdateLabel_WithInvalidId_ReturnsBadRequest()
    {
        // Arrange
        var updateRequest = new LabelRequest
        {
            Name = "Updated Label",
            ColorHex = "#0000FF",
            Type = "point",
            Shortcut = 'U',
            SubjectId = 1
        };

        var client = _client.WithScientistUser();

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
        var scientistId = Guid.NewGuid().ToString();
        var label = await DatabaseSeeder.SeedLabelAsync(context, null, scientistId);

        var client = _client.WithScientistUser(scientistId);

        // Act
        var response = await client.DeleteAsync($"/api/labels/{label.Id}");

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.NoContent);

        // Verify deletion (soft delete)
        var getResponse = await client.GetAsync($"/api/labels/{label.Id}");
        getResponse.StatusCode.Should().Be(HttpStatusCode.NotFound);
    }

    [Fact]
    public async Task DeleteLabel_WithInvalidId_ReturnsNotFound()
    {
        // Arrange
        var client = _client.WithScientistUser();

        // Act
        var response = await client.DeleteAsync("/api/labels/99999");

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.NotFound);
    }

    [Fact]
    public async Task DeleteLabel_AsLabeler_ReturnsForbidden()
    {
        // Arrange
        var context = await _factory.GetDbContextAsync();
        var label = await DatabaseSeeder.SeedLabelAsync(context);

        var client = _client.WithLabelerUser();

        // Act
        var response = await client.DeleteAsync($"/api/labels/{label.Id}");

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.Forbidden);
    }
}
