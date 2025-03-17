using ProjektGrupowy.API.Models;
using ProjektGrupowy.API.Utils;

namespace ProjektGrupowy.API.Repositories;

public interface ILabelerRepository
{
    Task<Optional<IEnumerable<Labeler>>> GetLabelersAsync();
    Task<Optional<Labeler>> GetLabelerAsync(int? id);
    Task<Optional<Labeler>> AddLabelerAsync(Labeler labeler);
    Task<Optional<Labeler>> UpdateLabelerAsync(Labeler labeler);
    Task DeleteLabelerAsync(Labeler labeler);

    Task<Optional<Labeler>> GetLabelerByUserIdAsync(string userId);

    Task<Optional<IEnumerable<Labeler>>> GetUnassignedLabelersOfProjectAsync(int projectId);
    Task<Optional<IEnumerable<Labeler>>> GetLabelersOfProjectAsync(int projectId);
}