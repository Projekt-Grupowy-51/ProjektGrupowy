using VidMark.Application.Features.Projects.Commands.AddProject;
using VidMark.Application.Interfaces.Repositories;
using VidMark.Application.Interfaces.UnitOfWork;
using VidMark.Domain.Models;

namespace VidMark.UnitTests.Features.Projects.Commands;

public class AddProjectCommandHandlerTests
{
    private readonly IProjectRepository _projectRepository;
    private readonly IUnitOfWork _unitOfWork;
    private readonly AddProjectCommandHandler _handler;

    public AddProjectCommandHandlerTests()
    {
        _projectRepository = Substitute.For<IProjectRepository>();
        _unitOfWork = Substitute.For<IUnitOfWork>();
        _handler = new AddProjectCommandHandler(_projectRepository, _unitOfWork);
    }

    [Fact]
    public async Task Handle_ValidCommand_CreatesAndReturnsProject()
    {
        // Arrange
        var command = new AddProjectCommand(
            Name: "Test Project",
            Description: "Test Description",
            Finished: false,
            UserId: "user123",
            IsAdmin: false
        );

        // Act
        var result = await _handler.Handle(command, CancellationToken.None);

        // Assert
        result.IsSuccess.Should().BeTrue();
        result.Value.Should().NotBeNull();
        result.Value.Name.Should().Be("Test Project");
        result.Value.Description.Should().Be("Test Description");
        result.Value.CreatedById.Should().Be("user123");
        result.Value.CreationDate.Should().Be(DateOnly.FromDateTime(DateTime.Today));

        await _projectRepository.Received(1).AddProjectAsync(Arg.Any<Project>());
        await _unitOfWork.Received(1).SaveChangesAsync(Arg.Any<CancellationToken>());
    }

    [Fact]
    public async Task Handle_ValidCommand_CallsRepositoryInCorrectOrder()
    {
        // Arrange
        var command = new AddProjectCommand(
            Name: "New Project",
            Description: "Description",
            Finished: false,
            UserId: "user456",
            IsAdmin: true
        );

        // Act
        await _handler.Handle(command, CancellationToken.None);

        // Assert
        Received.InOrder(async () =>
        {
            await _projectRepository.AddProjectAsync(Arg.Any<Project>());
            await _unitOfWork.SaveChangesAsync(Arg.Any<CancellationToken>());
        });
    }
}
