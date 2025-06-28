using ProjektGrupowy.Domain.Models;
using ProjektGrupowy.Domain.Utils;

namespace ProjektGrupowy.Infrastructure.Repositories;

public interface ILabelRepository
{
    Task<Optional<IEnumerable<Label>>> GetLabelsAsync(string userId, bool isAdmin);
    Task<Optional<Label>> GetLabelAsync(int id, string userId, bool isAdmin);
    Task<Optional<Label>> AddLabelAsync(Label label);
    Task<Optional<Label>> UpdateLabelAsync(Label label);
    Task DeleteLabelAsync(Label label);
}