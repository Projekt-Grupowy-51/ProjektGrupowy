using ProjektGrupowy.Domain.Models;
using ProjektGrupowy.Domain.Utils;

namespace ProjektGrupowy.Infrastructure.Repositories;

public interface ILabelRepository
{
    Task<Optional<IEnumerable<Label>>> GetLabelsAsync();
    Task<Optional<Label>> GetLabelAsync(int id);
    Task<Optional<Label>> AddLabelAsync(Label label);
    Task<Optional<Label>> UpdateLabelAsync(Label label);
    Task DeleteLabelAsync(Label label);
}